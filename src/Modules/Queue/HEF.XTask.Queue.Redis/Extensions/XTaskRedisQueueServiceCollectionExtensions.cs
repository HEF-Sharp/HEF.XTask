using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace HEF.XTask.Queue
{
    public static class XTaskRedisQueueServiceCollectionExtensions
    {
        public static IServiceCollection AddXTaskRedisQueue(
            this IServiceCollection services, Action<RedisQueueConfig> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddSingleton<IXTaskQueueProvider>(provider =>
            {
                var redisQueueConfig = provider.GetRequiredService<IOptions<RedisQueueConfig>>().Value;

                return new XTaskRedisQueueProvider(redisQueueConfig);
            });

            return services;
        }
    }
}
