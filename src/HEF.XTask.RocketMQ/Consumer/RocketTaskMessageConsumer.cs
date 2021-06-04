using HEF.MQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;
using System.Threading.Tasks;

namespace HEF.XTask.RocketMQ
{
    public abstract class RocketTaskMessageConsumer<TMessageBody> : IMQTypedMessageConsumer<MessageExt, RocketMessage<TMessageBody>>
    {
        protected RocketTaskMessageConsumer(IRocketTaskScheduler rocketTaskScheduler)
        {
            RocketTaskScheduler = rocketTaskScheduler ?? throw new ArgumentNullException(nameof(rocketTaskScheduler));
        }

        protected IRocketTaskScheduler RocketTaskScheduler { get; }

        public async Task<bool> Consume(MQTypedMessage<MessageExt, RocketMessage<TMessageBody>> typedMessage)
        {
            var rocketMessage = typedMessage.Content;

            if (rocketMessage.Delay.RemainDelaySeconds > 0)
            {
                //还没到延迟时间 通过剩余延迟秒数 发布新的延迟任务
                var nextDelayTask = new XRocketTask<TMessageBody>(rocketMessage, rocketMessage.Delay.RemainDelaySeconds);
                return RocketTaskScheduler.Schedule(nextDelayTask);
            }

            var result = await Consume(typedMessage.Message, rocketMessage);
            if (rocketMessage.Retry.IsRetrying)
            {
                rocketMessage.Retry.RetryOnce(); //累加重试次数
            }

            if (!result)  //延迟任务执行失败，进行重试
            {
                var retryTask = new XRocketTask<TMessageBody>(rocketMessage);
                RocketTaskScheduler.Retry(retryTask);
            }

            return true;
        }

        protected abstract Task<bool> Consume(MessageExt messageExt, RocketMessage<TMessageBody> rocketMessage);
    }
}
