namespace CLog.AspNetCore
{
    using System;

    public interface ICLoggerProvider : IDisposable
    {
        ILogger CreateLogger(string Name);
    }
}
