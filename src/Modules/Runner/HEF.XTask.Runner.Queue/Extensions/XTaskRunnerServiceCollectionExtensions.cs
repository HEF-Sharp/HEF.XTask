using Microsoft.Extensions.DependencyInjection;

namespace HEF.XTask.Runner
{
    public static class XTaskRunnerServiceCollectionExtensions
    {
        public static IServiceCollection AddXTaskReQueueCancelHandler<TParam>(this IServiceCollection services)
        {
            services.AddScoped<IXTaskCancelHandler<TParam>, ReQueueXTaskCancelHandler<TParam>>();

            return services;
        }

        public static IServiceCollection AddXTaskReQueueExceptionHandler<TParam>(this IServiceCollection services)
        {
            services.AddScoped<IXTaskExceptionHandler<TParam>, ReQueueXTaskExceptionHandler<TParam>>();

            return services;
        }
    }
}
