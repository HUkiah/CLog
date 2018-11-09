namespace CLog.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal sealed class MultiFileWatcher : IDisposable
    {
        private readonly Dictionary<string, FileSystemWatcher> _watcherMap = new Dictionary<string, FileSystemWatcher>();

        public NotifyFilters NotifyFilters { get; set; }

        public event FileSystemEventHandler FileChanged;

        public MultiFileWatcher() :
            this(NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes){ }

        public MultiFileWatcher(NotifyFilters notifyFilters)
        {
            NotifyFilters = notifyFilters;
        }

        public void Watch(IEnumerable<string> fileNames)
        {
            if (fileNames == null)
                return;
            foreach (string file in fileNames)
            {
                Watch(file);
            }
        }

        internal void Watch(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                //相关目录不存在
                return;
            }

            lock(this)
            {
                if (_watcherMap.ContainsKey(fileName))
                    return;

                FileSystemWatcher watcher = null;
                try
                {
                    watcher = new FileSystemWatcher
                    {
                        Path = directory,
                        Filter = Path.GetFileName(fileName),
                        NotifyFilter = NotifyFilters
                    };

                    watcher.Created += OnFileChanged;
                    watcher.Changed += OnFileChanged;
                    watcher.Deleted += OnFileChanged;
                    watcher.Renamed += OnFileChanged;
                    watcher.Error += OnWatcherError;
                    watcher.EnableRaisingEvents = true;

                    _watcherMap.Add(fileName, watcher);
                } catch (Exception ex)
                {
                    if (ex.MustBeRethrown())
                        throw;

                    if (watcher != null)
                        StopWatching(watcher);
                }
            }
        }

        public void StopWatching()
        {
            lock (this)
            {
                foreach (FileSystemWatcher watcher in _watcherMap.Values)
                {
                    StopWatching(watcher);
                }
                _watcherMap.Clear();
            }
        }

        private void StopWatching(FileSystemWatcher watcher)
        {
            try
            {
                watcher.EnableRaisingEvents = false;
                watcher.Created -= OnFileChanged;
                watcher.Changed -= OnFileChanged;
                watcher.Deleted -= OnFileChanged;
                watcher.Renamed -= OnFileChanged;
                watcher.Error -= OnWatcherError;
                watcher.Dispose();
            }
            catch(Exception ex)
            {
                if (ex.MustBeRethrown())
                    throw;
            }
        }

        private void OnWatcherError(object source,ErrorEventArgs e)
        {
            //日志记录CLog错误的发生
        }

        private void OnFileChanged(object source,FileSystemEventArgs e)
        {
            var changed = FileChanged;
            if (changed != null)
            {
                try
                {
                    changed(source, e);
                } catch (Exception ex)
                {
                    if (ex.MustBeRethrownImmediately())
                        throw;
                }
            }

        }

        public void Dispose()
        {
            FileChanged = null;
            //停止监控
            //
            GC.SuppressFinalize(this);
        }
    }
}
