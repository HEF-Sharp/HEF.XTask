using System;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.XTask.Queue
{
    public class XTaskRedisQueueProvider : IXTaskQueueProvider
    {
        private readonly RedisQueueConfig _queueConfig;

        public XTaskRedisQueueProvider(RedisQueueConfig queueConfig)
        {
            if (string.IsNullOrWhiteSpace(queueConfig?.ListKey))
                throw new ArgumentNullException(nameof(queueConfig.ListKey));

            if (string.IsNullOrWhiteSpace(queueConfig?.ConfirmListKey))
                throw new ArgumentNullException(nameof(queueConfig.ConfirmListKey));

            _queueConfig = queueConfig;
        }

        public async Task<bool> PushToRunTaskAsync<TParam>(XTask<TParam> toRunTask)
        {
            var listLength = await RedisHelper.LPushAsync(_queueConfig.ListKey, toRunTask);

            return listLength > 0;
        }

        public Task<XTask<TParam>> PopGetToRunTaskWithTimeoutAsync<TParam>(TimeSpan timeout, bool confirmCompleted)
        {
            if (confirmCompleted)
                return PopGetToRunTaskWithTimeoutAsync(timeout, () => RedisHelper.RPopLPushAsync<XTask<TParam>>(_queueConfig.ListKey, _queueConfig.ConfirmListKey));

            return PopGetToRunTaskWithTimeoutAsync(timeout, () => RedisHelper.RPopAsync<XTask<TParam>>(_queueConfig.ListKey));
        }

        public Task ConfirmToRunTaskCompletedAsync<TParam>(XTask<TParam> toRunTask)
        {
            return RedisHelper.LRemAsync(_queueConfig.ConfirmListKey, -1, toRunTask);
        }

        #region Helper Functions
        protected static Task<XTask<TParam>> PopGetToRunTaskWithTimeoutAsync<TParam>(TimeSpan timeout, Func<Task<XTask<TParam>>> popGetFunction)
        {
            return Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(timeout);

                var cancelToken = cts.Token;

                while (!cancelToken.IsCancellationRequested)
                {
                    var toRunTask = await popGetFunction.Invoke();

                    if (toRunTask != null)
                        return toRunTask;
                }

                return null;
            });
        }
        #endregion
    }
}
