namespace CLog.AspNetCore
{
    using Microsoft.Extensions.DependencyInjection;

    public interface ICLogBuilder
    {
        IServiceCollection Services { get; }
    }
}
