using NewLife.RocketMQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskScheduler
    {
        bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask);

        bool Retry<TMessageBody>(XRocketTask<TMessageBody> rocketTask);
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

            var rocketMessage = rocketTask.Params;

            //构造Delay信息
            rocketMessage.Delay = DelayProvider.CreateRocketDelay(rocketTask.DelaySeconds);

            return PublishRocketMessage(rocketMessage);
        }

        public bool Retry<TMessageBody>(XRocketTask<TMessageBody> rocketTask)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            var rocketMessage = rocketTask.Params;

            rocketMessage.Delay = GetRetryRocketDelay(rocketMessage);
            rocketMessage.RetryCount++;

            return PublishRocketMessage(rocketMessage);
        }

        #region Helper Functions
        private bool PublishRocketMessage<TMessageBody>(RocketMessage<TMessageBody> rocketMessage)
        {
            var typedProducer = ProducerProvider.GetTypedProducer(rocketMessage.Dispatch.Topic);

            var result = typedProducer.Publish(rocketMessage,
                b => b.Tag(rocketMessage.Dispatch.Tag).Delay(rocketMessage.Delay.DelayTimeLevel));

            return result.Status == SendStatus.SendOK;
        }

        private RocketDelay GetRetryRocketDelay<TMessageBody>(RocketMessage<TMessageBody> rocketMessage)
        {
            if (rocketMessage.RetryCount == 0)
                return DelayProvider.GetMinRocketDelay();

            return DelayProvider.GetNextRocketDelay(rocketMessage.Delay.DelayTimeLevel);
        }
        #endregion
    }
}
