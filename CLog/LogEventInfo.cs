using System;

namespace CLog
{
    public class LogEventInfo
    {
        public string LoggerName { get; set; }

        public LogLevel Level { get; set; }


        public static LogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatprovider, object message)
        {
            Exception exception = message as Exception;
            if (exception == null && message is LogEventInfo logEvent)
            {
                logEvent.LoggerName = loggerName;
                logEvent.Level = logLevel;
                logLevel.FormatProvider = formatprovider ?? logEvent.FormatProvider;

                return logLevel;
            }
            return new LogEventInfo();
        }
    }
}
