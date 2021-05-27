using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.XTask.RocketMQ
{
    public static class RocketTaskServiceCollectionExtensions
    {
        public static IServiceCollection AddRocketTask(this IServiceCollection collection,
            Func<IServiceProvider, IRocketDelayProvider> delayProviderFactory)
        {
            if (delayProviderFactory == null)
                throw new ArgumentNullException(nameof(delayProviderFactory));

            collection.AddSingleton(delayProviderFactory);
            collection.AddSingleton<IRocketTaskScheduler, RocketTaskScheduler>();

            return collection;
        }
    }
}
