using HEF.XTask.Queue;
using System;
using System.Threading.Tasks;

namespace HEF.XTask.Runner
{
    public class ReQueueXTaskCancelHandler<TParam> : IXTaskCancelHandler<TParam>
    {
        public ReQueueXTaskCancelHandler(IXTaskQueueProvider taskQueueProvider)
        {
            QueueProvider = taskQueueProvider ?? throw new ArgumentNullException(nameof(taskQueueProvider));
        }

        protected IXTaskQueueProvider QueueProvider { get; }

        public Task HandleCancelledTaskAsync(XTask<TParam> cancelledTask)
        {
            return QueueProvider.PushToRunTaskAsync(cancelledTask);
        }
    }
}
