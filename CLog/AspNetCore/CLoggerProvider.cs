namespace CLog.AspNetCore
{
    using Microsoft.Extensions.Options;

    public class CLoggerProvider:ICLoggerProvider
    {
        public CLoggerProvider(IOptions<CLoggerOptions> options)
        {
            var loggerOptions = options.Value;

        }

        public ILogger CreateLogger(string Name)
        {
            return LogManager.GetCurrentClassLogger();
        }

        public void Dispose()
        {
            
        }
    }
}
