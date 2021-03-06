﻿namespace CLog.Config
{
    using CLog.Internal;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class LoggingConfiguration
    {
        public LogLevel LogLevel = LogLevel.Trace;

        private long? _fileSizeLimit = 1L * 2048 * 2048 * 2048;
        private int? _retainedFileCountLimit = 31;
        private string _LogDirectory = "logs";
        private string _fileName = "log";

        public long? FileSizeLimit
        {
            get { return _fileSizeLimit; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FileSizeLimit)}日志文件大小必须大于0");

                _fileSizeLimit = value;
            }
        }

        public int? RetainedFileCountLimit
        {
            get { return _retainedFileCountLimit; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(RetainedFileCountLimit)} 日志文件最大数量应大于 0");
                _retainedFileCountLimit = value;
            }
        }

        public string LogDirectory
        {
            get { return _LogDirectory; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(nameof(value));
                _LogDirectory = value;
            }
        }

        public string Filename
        {
            get { return _fileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(nameof(value));
                _fileName = value;
            }
        }

        public virtual IEnumerable<string> FileNamesWatch => ArrayHelper.Empty<string>();

        public virtual LoggingConfiguration Reload()
        {
            return this;
        }

        private List<object> _configItems = new List<object>();

        internal void Close()
        {
           
        }

        private List<ISupportInitialize> GetSupportsInitializes(bool reverse = false)
        {
            var items = _configItems.OfType<ISupportInitialize>();
            if (reverse)
            {
                items = items.Reverse();
            }
            return items.ToList();
        }
    }
}
