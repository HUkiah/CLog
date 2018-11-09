namespace CLog
{
    using System;

    public class CLogConfigurationException:Exception
    {
        public CLogConfigurationException(string message, Exception innerException):base(message,innerException)
        { }

        public CLogConfigurationException(string message) : base(message) { }
    }
}
