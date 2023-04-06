using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.XTask.Hosting
{
    public static class XTaskHostingServiceCollectionExtensions
    {
        public static IServiceCollection AddXTaskHosting<TParam>(
            this IServiceCollection services, Action<XTaskScheduleConfig> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddHostedService<XTaskScheduleHostedService<TParam>>();

            return services;
        }
    }
}
