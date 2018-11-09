namespace CLog.AspNetCore
{
    using Microsoft.Extensions.Configuration;


    public interface ILoggerProviderConfiguration
    {
        IConfiguration Configuration { get; }
    }
}
