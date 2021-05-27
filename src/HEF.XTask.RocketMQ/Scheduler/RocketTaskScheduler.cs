using NewLife.RocketMQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskScheduler
    {
        bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask);
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

            //构造Delay信息
            rocketTask.Params.Delay = DelayProvider.CreateRocketDelay(rocketTask.DelaySeconds);

            var typedProducer = ProducerProvider.GetTypedProducer(rocketTask.Params.Dispatch.Topic);

            var result = typedProducer.Publish(rocketTask.Params,
                b => b.Tag(rocketTask.Params.Dispatch.Tag).Delay(rocketTask.Params.Delay.DelayTimeLevel));

            return result.Status == SendStatus.SendOK;
        }
    }
}
