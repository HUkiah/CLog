using System;
using System.Collections.Generic;
using System.Text;

namespace CLog.Config
{
    public class LoggingConfigurationChangedEventArgs:EventArgs
    {
        public LoggingConfiguration DeactivatedConfiguration { get; private set; }

        public LoggingConfiguration ActivatedConfiguration { get; private set; }

        public LoggingConfigurationChangedEventArgs(LoggingConfiguration deactivatedConfiguration, LoggingConfiguration activatedConfiguration)
        {
            DeactivatedConfiguration = deactivatedConfiguration;
            ActivatedConfiguration = activatedConfiguration;
        }


    }
}
