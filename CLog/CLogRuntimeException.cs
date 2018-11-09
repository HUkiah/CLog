namespace CLog
{
    using System;

    public class CLogRuntimeException:Exception
    {
        public CLogRuntimeException(string message) : base(message)
        { }
    }
}
