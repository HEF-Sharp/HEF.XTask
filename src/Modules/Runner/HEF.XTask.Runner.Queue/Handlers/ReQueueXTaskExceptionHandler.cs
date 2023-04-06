using HEF.XTask.Queue;
using System;
using System.Threading.Tasks;

namespace HEF.XTask.Runner
{
    public class ReQueueXTaskExceptionHandler<TParam> : IXTaskExceptionHandler<TParam>
    {
        public ReQueueXTaskExceptionHandler(IXTaskQueueProvider taskQueueProvider)
        {
            QueueProvider = taskQueueProvider ?? throw new ArgumentNullException(nameof(taskQueueProvider));
        }

        protected IXTaskQueueProvider QueueProvider { get; }

        public Task HandleExceptionTaskAsync(XTask<TParam> exceptionTask, Exception exception)
        {
            return QueueProvider.PushToRunTaskAsync(exceptionTask);
        }
    }
}
