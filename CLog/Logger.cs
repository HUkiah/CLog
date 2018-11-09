namespace CLog
{
    using CLog.Internal;
    using System;

    public partial class Logger:ILogger
    {
        private volatile LoggerConfiguration _configuration;
        private volatile bool _isTraceEnabled;
        private volatile bool _isDebugEnabled;
        private volatile bool _isInfoEnabled;
        private volatile bool _isWarnEnabled;
        private volatile bool _isErrorEnabled;
        private volatile bool _isFatalEnabled;

        public string Name { get; private set; }

        public LogFactory Factory { get;private set; }

        public event EventHandler<EventArgs> LoggerReconfigured;

        protected internal Logger()
        { }

        internal void Initialize(string name, LoggerConfiguration loggerConfiguration, LogFactory logFactory)
        {
            Name = name;
            Factory = logFactory;
            SetConfiguration(loggerConfiguration);
        }

        internal void SetConfiguration(LoggerConfiguration newConfiguration)
        {
            _configuration = newConfiguration;

            //LogLevel 的一些设置
            _isTraceEnabled = newConfiguration.IsEnabled(LogLevel.Trace);
            _isDebugEnabled = newConfiguration.IsEnabled(LogLevel.Debug);
            _isInfoEnabled = newConfiguration.IsEnabled(LogLevel.Info);
            _isWarnEnabled = newConfiguration.IsEnabled(LogLevel.Warn);
            _isErrorEnabled = newConfiguration.IsEnabled(LogLevel.Error);
            _isFatalEnabled = newConfiguration.IsEnabled(LogLevel.Fatal);

            OnLoggerReconfigured(EventArgs.Empty);
        }

        protected virtual void OnLoggerReconfigured(EventArgs e)
        {
            LoggerReconfigured?.Invoke(this, e);
        }

        
    }
}
