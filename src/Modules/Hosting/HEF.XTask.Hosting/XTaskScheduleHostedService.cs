using HEF.XTask.Queue;
using HEF.XTask.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.XTask.Hosting
{
    public class XTaskScheduleHostedService<TParam> : BackgroundService
    {
        private readonly XTaskScheduleConfig _taskScheduleConfig;

        private readonly List<Task> _runningTasks = new List<Task>();

        public XTaskScheduleHostedService(IServiceScopeFactory scopeFactory,
            IOptions<XTaskScheduleConfig> taskScheduleOptions)
        {
            ScopeFactory = scopeFactory;

            _taskScheduleConfig = taskScheduleOptions.Value;
        }

        protected IServiceScopeFactory ScopeFactory { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            await ProcessingStartToRunTaskAsync(stoppingToken);
        }

        protected async Task ProcessingStartToRunTaskAsync(CancellationToken stoppingToken)
        {
            using var scope = ScopeFactory.CreateScope();
            var taskQueueProvider = scope.ServiceProvider.GetRequiredService<IXTaskQueueProvider>();

            //检查是否存在Cancel退出的 队列任务
            await CheckHandleRunTaskCancelledAsync(taskQueueProvider);

            while (!stoppingToken.IsCancellationRequested)
            {
                var toRunTask = await GetToRunTaskWithTimeoutAsync(taskQueueProvider);

                if (toRunTask != null)
                {
                    var runningTask = Task.Run(() => StartRunTaskAsync(toRunTask, stoppingToken));
                    _runningTasks.Add(runningTask);

                    //Task运行结束后 确认任务已完成
                    _ = runningTask.ContinueWith(async task =>
                    {
                        await ConfirmToRunTaskCompletedAsync(taskQueueProvider, toRunTask);

                        if (!task.IsCompletedSuccessfully)  //异常运行结束的Task
                            await HandleRunTaskExceptionAsync(toRunTask);

                        _runningTasks.Remove(task);
                    });
                }
            }

            //stoppingToken cancel时 等待所有运行中的Task结束执行
            await Task.WhenAll(_runningTasks.ToArray());
        }

        protected virtual async Task CheckHandleRunTaskCancelledAsync(IXTaskQueueProvider taskQueueProvider)
        {
            while (true)
            {
                var waitConfirmTask = await taskQueueProvider.PopGetWaitConfirmTaskAsync<TParam>();

                if (waitConfirmTask is null)
                    break;

                //Cancel结束的RunningTask
                using var scope = ScopeFactory.CreateScope();
                var cancelHandler = scope.ServiceProvider.GetRequiredService<IXTaskCancelHandler<TParam>>();

                await cancelHandler.HandleCancelledTaskAsync(waitConfirmTask);
            }
        }

        protected virtual Task<XTask<TParam>> GetToRunTaskWithTimeoutAsync(IXTaskQueueProvider taskQueueProvider)
        {
            var popTimeout = TimeSpan.FromSeconds(_taskScheduleConfig.TaskQueueTimeoutSeconds);

            return taskQueueProvider.PopGetToRunTaskWithTimeoutAsync<TParam>(popTimeout, true);
        }

        protected virtual Task ConfirmToRunTaskCompletedAsync(IXTaskQueueProvider taskQueueProvider, XTask<TParam> toRunTask)
        {
            return taskQueueProvider.ConfirmTaskCompletedAsync(toRunTask);
        }

        protected virtual Task<bool> StartRunTaskAsync(XTask<TParam> toRunTask, CancellationToken stoppingToken)
        {
            using var scope = ScopeFactory.CreateScope();
            var taskExecutor = scope.ServiceProvider.GetRequiredService<IXTaskExecutor<TParam>>();

            return taskExecutor.ExecuteTaskAsync(toRunTask, stoppingToken);
        }

        protected virtual Task HandleRunTaskExceptionAsync(XTask<TParam> exceptionTask)
        {
            using var scope = ScopeFactory.CreateScope();
            var exceptionHandler = scope.ServiceProvider.GetRequiredService<IXTaskExceptionHandler<TParam>>();

            return exceptionHandler.HandleExceptionTaskAsync(exceptionTask);
        }
    }
}
