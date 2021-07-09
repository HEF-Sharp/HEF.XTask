using NewLife.RocketMQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskScheduler
    {
        bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask);

        bool Retry<TMessageBody>(XRocketTask<TMessageBody> rocketTask);

        bool Timing<TMessageBody>(XRocketTask<TMessageBody> rocketTask);
    }

    public class RocketTaskScheduler : IRocketTaskScheduler
    {
        public RocketTaskScheduler(IRocketDelayProvider delayProvider,
            IRocketMQProducerProvider producerProvider)
        {
            DelayProvider = delayProvider ?? throw new ArgumentNullException(nameof(delayProvider));
            ProducerProvider = producerProvider ?? throw new ArgumentNullException(nameof(producerProvider));
        }

        protected IRocketDelayProvider DelayProvider { get; }

        protected IRocketMQProducerProvider ProducerProvider { get; }

        public bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            var rocketMessage = rocketTask.InnerTask.Params;

            //构造Delay信息
            rocketMessage.Delay = DelayProvider.CreateRocketDelay(rocketTask.DelaySeconds);

            return PublishRocketMessage(rocketMessage);
        }

        public bool Retry<TMessageBody>(XRocketTask<TMessageBody> rocketTask)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            var rocketMessage = rocketTask.InnerTask.Params;

            if (rocketMessage.Retry.IsRetryEnd())
                return false;  //已达最大重试次数，结束重试

            if (rocketMessage.Retry.IsRetrying)
            {
                rocketMessage.Delay = GetNextRocketDelay(rocketMessage);
                return PublishRocketMessage(rocketMessage);
            }

            if (!rocketMessage.Retry.StartRetry())
                return false;   //启动重试失败(最大重试次数小于1)，不需要重试

            //发布首次重试任务
            rocketMessage.Delay = DelayProvider.GetMinRocketDelay();
            return PublishRocketMessage(rocketMessage);
        }

        public bool Timing<TMessageBody>(XRocketTask<TMessageBody> rocketTask)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            var rocketMessage = rocketTask.InnerTask.Params;

            throw new NotImplementedException();
        }

        #region Helper Functions
        private bool PublishRocketMessage<TMessageBody>(RocketMessage<TMessageBody> rocketMessage)
        {
            var typedProducer = ProducerProvider.GetTypedProducer(rocketMessage.Dispatch.Topic);

            var result = typedProducer.Publish(rocketMessage,
                b => b.Tag(rocketMessage.Dispatch.Tag).Delay(rocketMessage.Delay.DelayTimeLevel));

            return result.Status == SendStatus.SendOK;
        }

        private RocketDelay GetNextRocketDelay<TMessageBody>(RocketMessage<TMessageBody> rocketMessage)
        {
            return DelayProvider.GetNextRocketDelay(rocketMessage.Delay.DelayTimeLevel);
        }
        #endregion
    }
}
