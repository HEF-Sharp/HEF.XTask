using HEF.XTask.Queue;
using HEF.XTask.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.XTask.Hosting
{
    public static class XTaskHostingServiceCollectionExtensions
    {
        public static IServiceCollection AddXTaskHosting<TParam>(
            this IServiceCollection services, Action<IXTaskHostingConfigurator> configure)
        {
            XTaskHostingConfigurator hostingConfigurator = new XTaskHostingConfigurator(services);
            configure?.Invoke(hostingConfigurator);

            services.AddHostedService<XTaskScheduleHostedService<TParam>>();

            return services;
        }

        public static IXTaskHostingConfigurator Schedule(
            this IXTaskHostingConfigurator hostingConfigurator, Action<XTaskScheduleConfig> configureOptions)
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.Configure(configureOptions);

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator UsingQueue(
            this IXTaskHostingConfigurator hostingConfigurator, Func<IServiceProvider, IXTaskQueueProvider> queueProviderFactory)
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddSingleton(queueProviderFactory);

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator UsingQueue<TXTaskQueueProvider>(
            this IXTaskHostingConfigurator hostingConfigurator)
            where TXTaskQueueProvider : class, IXTaskQueueProvider
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddSingleton<IXTaskQueueProvider, TXTaskQueueProvider>();

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator WithExecutor<TParam>(
            this IXTaskHostingConfigurator hostingConfigurator, Func<IServiceProvider, IXTaskExecutor<TParam>> executorFactory)
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped(executorFactory);

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator WithExecutor<TParam, TXTaskExecutor>(
            this IXTaskHostingConfigurator hostingConfigurator)
            where TXTaskExecutor : class, IXTaskExecutor<TParam>
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped<IXTaskExecutor<TParam>, TXTaskExecutor>();

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator HandleException<TParam>(
            this IXTaskHostingConfigurator hostingConfigurator, Func<IServiceProvider, IXTaskExceptionHandler<TParam>> exceptionHandlerFactory)
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped(exceptionHandlerFactory);

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator HandleException<TParam, TXTaskExceptionHandler>(
            this IXTaskHostingConfigurator hostingConfigurator)
            where TXTaskExceptionHandler : class, IXTaskExceptionHandler<TParam>
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped<IXTaskExceptionHandler<TParam>, TXTaskExceptionHandler>();

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator HandleCancel<TParam>(
            this IXTaskHostingConfigurator hostingConfigurator, Func<IServiceProvider, IXTaskCancelHandler<TParam>> cancelHandlerFactory)
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped(cancelHandlerFactory);

            return hostingConfigurator;
        }

        public static IXTaskHostingConfigurator HandleCancel<TParam, TXTaskCancelHandler>(
            this IXTaskHostingConfigurator hostingConfigurator)
            where TXTaskCancelHandler : class, IXTaskCancelHandler<TParam>
        {
            if (hostingConfigurator == null)
                throw new ArgumentException(nameof(hostingConfigurator));

            hostingConfigurator.Services.AddScoped<IXTaskCancelHandler<TParam>, TXTaskCancelHandler>();

            return hostingConfigurator;
        }
    }
}
