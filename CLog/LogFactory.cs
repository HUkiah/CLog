namespace CLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using CLog.Common;
    using CLog.Config;
    using CLog.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class LogFactory : IDisposable
    {
        internal readonly object _syncRoot = new object();
        private readonly LoggerCache _loggerCache = new LoggerCache();
        /// <summary>
        /// 获取或设置一个值，标识是否应该抛出异常
        /// </summary>
        public bool ThrowExceptions { get; set; }
        public bool? ThrowConfigExceptions { get; set; }

        //配置文件更改后，重新检测加载的时间
        private const int ReConfigAfterFileChangedTimeout = 1000;
        internal Timer _reloaderTimer;
        private readonly MultiFileWatcher _watcher;
        private LoggingConfiguration _config;
        private bool _configLoaded;

        public event EventHandler<LoggingConfigurationReloadedEventArgs> ConfigurationReloaded;
        public event EventHandler<LoggingConfigurationChangedEventArgs> ConfigurationChanged;

        public LogFactory()
        {
            _watcher = new MultiFileWatcher();
            _watcher.FileChanged += ConfigFileChanged;
            
        }

        //可以再GET属性中配置读取文件

        public LoggingConfiguration Configuration
        {
            get
            {
                if (_configLoaded)
                    return _config;

                lock (_syncRoot)
                {
                    if (_configLoaded)
                        return _config;

                    if (_config == null)
                    {
                        TryLoadFromConfigFile(out _config);
                    }

                    if (_config != null)
                    {
                        try
                        {
                            ReconfigExistingLoggers();
                            TryWachtingConfigFile();
                        } finally
                        {
                            _configLoaded = true;
                        }
                    }

                    return _config;
                }
            }
            set
            {
                try
                {
                    _watcher.StopWatching();
                }
                catch (Exception ex)
                {
                    if (ex.MustBeRethrown())
                        throw;
                }

                lock (_syncRoot)
                {
                    LoggingConfiguration oldConfig = _config;
                    if (oldConfig != null)
                    {
                        Flush();
                        oldConfig.Close();
                    }

                    _config = value;
                    if (_config == null)
                        _configLoaded = false;
                    else
                    {
                        try
                        {
                            ReconfigExistingLoggers();
                            TryWachtingConfigFile();
                        }
                        finally
                        {
                            _configLoaded = true;
                        }
                    }

                    OnConfigurationChanged(new LoggingConfigurationChangedEventArgs(value, oldConfig));
                }
            }
        }

        protected virtual void OnConfigurationChanged(LoggingConfigurationChangedEventArgs e)
        {
            ConfigurationChanged?.Invoke(this, e);
        }

        public void Flush()
        {

        }


        private void TryWachtingConfigFile()
        {
            try
            {
                _watcher.Watch(_config.FileNamesWatch);
            } catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                    throw;

            }
        }

        public void ReconfigExistingLoggers()
        {
            List<Logger> loggers;

            lock(_syncRoot)
            {
                loggers = _loggerCache.GetLogger();
            }

            foreach (var logger in loggers)
            {
                logger.SetConfiguration(GetConfigurationForLogger(logger.Name, _config));
            }
        }

        private bool TryLoadFromConfigFile(out LoggingConfiguration config)
        {
            try
            {
                var Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

                config = new ServiceCollection()
                    .AddOptions()
                    .Configure<LoggingConfiguration>(Configuration.GetSection("Logging"))
                    .BuildServiceProvider()
                    .GetService<IOptions<LoggingConfiguration>>()
                    .Value;

                return true;
            } catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                    throw;
            }
            config = null;
            return false;
        }


        private void ConfigFileChanged(object sender, EventArgs args)
        {
            //Configuration file change deteced! Reload in {ReconfigAfterFileChangedTimeout}
            lock (_syncRoot)
            {
                if (_reloaderTimer == null)
                {
                    var configuration = Configuration;
                    if (Configuration != null)
                    {
                        _reloaderTimer = new Timer(
                            ReloadConfigOnTimer,
                            configuration,
                            ReConfigAfterFileChangedTimeout,
                            Timeout.Infinite
                            );
                    }
                }
                else
                {
                    _reloaderTimer.Change(ReConfigAfterFileChangedTimeout, Timeout.Infinite);
                }

            }
        }

        internal void ReloadConfigOnTimer(object state)
        {
            if (_reloaderTimer == null)
                return;

            LoggingConfiguration configurationToReload = (LoggingConfiguration)state;
            lock (_syncRoot)
            {
                try
                {
                    var currentTimer = _reloaderTimer;
                    if (currentTimer != null)
                    {
                        _reloaderTimer = null;
                        currentTimer.WaitForDispose(TimeSpan.Zero);
                    }

                    _watcher.StopWatching();

                    if (_config != configurationToReload)
                    {
                        throw new CLogConfigurationException("配置在两者之间改变，不重新加载");
                    }

                    LoggingConfiguration newConfig = configurationToReload.Reload();

                    if (newConfig != null)
                    {
                        Configuration = newConfig;
                        OnConfigurationReloaded(new LoggingConfigurationReloadedEventArgs(true));
                    }
                    else
                    {
                        throw new CLogConfigurationException("Configuration.Reload() 返回空值.不能加载");
                    }

                } catch (Exception ex)
                {
                    if (ex.MustBeRethrownImmediately())
                        throw;
                    _watcher.Watch(configurationToReload.FileNamesWatch);
                    OnConfigurationReloaded(new LoggingConfigurationReloadedEventArgs(false, ex));
                }
            }
        }

        protected virtual void OnConfigurationReloaded(LoggingConfigurationReloadedEventArgs e)
        {
            ConfigurationReloaded?.Invoke(this, e);
        }

        public Logger GetLogger(string name)
        {
            return GetLogger(new LoggerCacheKey(name, typeof(Logger)));
        }

        private Logger GetLogger(LoggerCacheKey cacheKey)
        {
            lock (_syncRoot)
            {
                Logger existingLogger = _loggerCache.Retrieve(cacheKey);
                if (existingLogger != null)
                {
                    return existingLogger;
                }

                Logger newLogger;

                //构建Logger对象
                if (cacheKey.ConcreteType != null && cacheKey.ConcreteType != typeof(Logger))
                {
                    var fullName = cacheKey.ConcreteType.FullName;
                    try
                    {
                        if (cacheKey.ConcreteType.IsStaticClass())
                        {
                            var errorMessage =
                                $"创建一个静态类 '{fullName}' 的实例是不可能的,它也不应该继承Logger ";
                            if (ThrowExceptions)
                            {
                                throw new CLogRuntimeException(errorMessage);
                            }
                            newLogger = CreateDefaultLogger(ref cacheKey);
                        }
                        else
                        {
                            var instance = FactoryHelper.CreateInstance(cacheKey.ConcreteType);
                            newLogger = instance as Logger;
                            if(newLogger == null)
                            {
                                var errorMessage =
                                    $"不能创建类型为'{fullName}'的实例。它应该有一个默认的构造函数。";
                                if (ThrowExceptions)
                                {
                                    throw new CLogRuntimeException(errorMessage);
                                }
                                //如果无法创建指定类型的实例，则创建logger的默认实例。
                                newLogger = CreateDefaultLogger(ref cacheKey);
                            }
                        }
                    } catch (Exception ex)
                    {
                        if (ex.MustBeRethrown())
                        {
                            throw;
                        }

                        //如果无法创建指定类型的实例，则创建logger的默认实例。
                        newLogger = CreateDefaultLogger(ref cacheKey);
                    }
                }
                else
                {
                    newLogger = new Logger();
                }

                if (cacheKey.ConcreteType != null)
                {
                    newLogger.Initialize(cacheKey.Name, GetConfigurationForLogger(cacheKey.Name,Configuration), this);
                }
                return newLogger;
            }
        }

        internal LoggerConfiguration GetConfigurationForLogger(string name, LoggingConfiguration configuration)
        {
            ///构建LoggerConfiguration

            return new LoggerConfiguration(configuration.LogLevel);
        }

        private static Logger CreateDefaultLogger(ref LoggerCacheKey cacheKey)
        {
            cacheKey = new LoggerCacheKey(cacheKey.Name, typeof(Logger));

            var newLogger = new Logger();
            return newLogger;
        }

        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class LoggerCache
        {
            private readonly Dictionary<LoggerCacheKey, WeakReference> _loggerCache = new Dictionary<LoggerCacheKey, WeakReference>();

            public Logger Retrieve(LoggerCacheKey loggerCacheKey)
            {
                if (_loggerCache.TryGetValue(loggerCacheKey, out var loggerReference))
                {
                    return loggerReference.Target as Logger;
                }

                return null;
            }

            public List<Logger> GetLogger()
            {
                List<Logger> values = new List<Logger>(_loggerCache.Count);

                foreach (var loggerReference in _loggerCache.Values)
                {
                    if (loggerReference.Target is Logger logger)
                    {
                        values.Add(logger);
                    }
                }

                return values;
            }
        }

        internal struct LoggerCacheKey : IEquatable<LoggerCacheKey>
        {
            public readonly string Name;
            public readonly Type ConcreteType;

            public LoggerCacheKey(string name, Type concreteType)
            {
                Name = name;
                ConcreteType = concreteType;
            }

            public override int GetHashCode()
            {
                return ConcreteType.GetHashCode() ^ Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is LoggerCacheKey Key && Equals(Key);
            }

            public bool Equals(LoggerCacheKey other)
            {
                return (ConcreteType == other.ConcreteType) && string.Equals(other.Name, Name, StringComparison.Ordinal);
            }
        }

    }


}
