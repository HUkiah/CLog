using System;
using System.Collections.Generic;
using System.Text;

namespace CLog
{
    public partial  class Logger
    {
        public void Debug(string message)
        {
            if (_isDebugEnabled)
            {

            }
        }

        private void WriteToTargets<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            var logLevel = PrepareLogEventInfo(LogEventInfo)
        }
    }
}
