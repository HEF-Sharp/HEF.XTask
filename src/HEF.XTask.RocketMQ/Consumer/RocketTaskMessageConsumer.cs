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

        public Task<bool> Consume(MQTypedMessage<MessageExt, RocketMessage<TMessageBody>> typedMessage)
        {
            var rocketMessage = typedMessage.Content;

            if (rocketMessage.Delay.RemainDelaySeconds > 0)
            {
                //还没到延迟时间 通过剩余延迟秒数 发布新的延迟任务
                var nextDelayTask = new XRocketTask<TMessageBody>(rocketMessage, rocketMessage.Delay.RemainDelaySeconds);

                var isPublished = RocketTaskScheduler.Schedule(nextDelayTask);
                return Task.FromResult(isPublished);
            }

            return Consume(typedMessage.Message, rocketMessage);
        }

        protected abstract Task<bool> Consume(MessageExt messageExt, RocketMessage<TMessageBody> rocketMessage);
    }
}
