using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Config
{
    public class LoggingConfigurationReloadedEventArgs:EventArgs
    {
        public bool Succeeded { get; private set; }

        public Exception Exception { get; private set; }

        public LoggingConfigurationReloadedEventArgs(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public LoggingConfigurationReloadedEventArgs(bool succeeded, Exception exception) : this(succeeded)
        {
            Exception = exception;
        }
    }
}
