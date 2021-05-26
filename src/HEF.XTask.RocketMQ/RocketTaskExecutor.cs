using NewLife.RocketMQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;
using System.Threading.Tasks;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskExecutor
    {
        Task<bool> Execute<TMessageBody>(XRocketTask<TMessageBody> rocketTask);
    }

    public class RocketTaskExecutor : IRocketTaskExecutor
    {
        public RocketTaskExecutor(IRocketMQProducerProvider rocketProducerProvider)
        {
            ProducerProvider = rocketProducerProvider ?? throw new ArgumentNullException(nameof(rocketProducerProvider));
        }

        protected IRocketMQProducerProvider ProducerProvider { get; }

        public async Task<bool> Execute<TMessageBody>(XRocketTask<TMessageBody> rocketTask)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            if (rocketTask.Params == null)
                throw new ArgumentNullException(nameof(rocketTask.Params));

            if (rocketTask.Dispatch == null)
                throw new ArgumentNullException(nameof(rocketTask.Dispatch));

            await Task.Yield();

            var typedProducer = ProducerProvider.GetTypedProducer(rocketTask.Dispatch.Topic);

            var result = typedProducer.Publish(rocketTask.Params,
                b => b.Tag(rocketTask.Dispatch.Tag).Delay(rocketTask.Params.Delay.DelayTimeLevel));

            return result.Status == SendStatus.SendOK;
        }
    }
}
