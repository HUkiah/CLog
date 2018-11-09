using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Internal
{
    internal class LoggerConfiguration
    {
        public LogLevel LogLevel { get; private set; }

        public bool IsEnabled(LogLevel level)
        {
            if (level == LogLevel)
            {
                return false;
            }
            return level >= LogLevel;
        }

        public LoggerConfiguration(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }
    }
}
