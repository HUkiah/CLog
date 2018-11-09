namespace CLog.AspNetCore
{
    using Microsoft.Extensions.Logging.Configuration;
    using Microsoft.Extensions.Options;

    public class CLoggerOptionsSetup:ConfigureFromConfigurationOptions<CLoggerOptions>
    {
        public CLoggerOptionsSetup(ILoggerProviderConfiguration<CLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration)
        { }
    }
}
