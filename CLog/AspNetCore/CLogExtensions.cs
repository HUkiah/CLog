namespace CLog.AspNetCore
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using System;

    public static class CLogExtensions
    {
        public static IServiceCollection AddCLog(this IServiceCollection services,IConfiguration configuration = null)
        {
            if (configuration == null)
                throw new ArgumentOutOfRangeException("Configuration 为空，不能启用配置");

            services.Configure<CLoggerOptions>(configuration);
            services.TryAdd(ServiceDescriptor.Singleton<ICLoggerProvider, CLoggerProvider>());
            services.AddSingleton<IConfigureOptions<CLoggerOptions>, CLoggerOptionsSetup>();
            services.AddSingleton<IOptionsChangeTokenSource<CLoggerOptions>, ConfigurationChangeTokenSource<CLoggerOptions>>();

            return services;
        }

        public static IServiceCollection AddCLog(this IServiceCollection services,Action<CLoggerOptions> configure =null)
        {
            if (configure!=null)
            {
                services.Configure(configure);
            }
            services.TryAdd(ServiceDescriptor.Singleton<ICLoggerProvider, CLoggerProvider>());
            return services;
        }

        public static ICLogBuilder AddCLog(this ICLogBuilder builder)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ICLoggerProvider, CLoggerProvider>());

            return builder;
        }

        public static ICLogBuilder AddCLog(this ICLogBuilder builder, Action<CLoggerOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            builder.AddCLog();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
