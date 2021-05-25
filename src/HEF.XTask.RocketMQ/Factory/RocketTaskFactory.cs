using System;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskFactory
    {
        XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic);

        XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, string tag);

        XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, int delaySeconds);

        XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, string tag, int delaySeconds);
    }

    public class RocketTaskFactory : IRocketTaskFactory
    {
        public RocketTaskFactory(IRocketDelayProvider delayProvider)
        {
            DelayProvider = delayProvider ?? throw new ArgumentNullException(nameof(delayProvider));
        }

        protected IRocketDelayProvider DelayProvider { get; }

        public XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic)
            => CreateRocketTask(messageBody, topic, null);

        public XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, string tag)
            => CreateRocketTask(messageBody, topic, tag, 0);

        public XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, int delaySeconds)
            => CreateRocketTask(messageBody, topic, null, delaySeconds);        

        public XRocketTask<TMessageBody> CreateRocketTask<TMessageBody>(TMessageBody messageBody, string topic, string tag, int delaySeconds)
        {
            if (messageBody == null)
                throw new ArgumentNullException(nameof(messageBody));

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException(nameof(topic));

            var rocketMessage = new RocketMessage<TMessageBody> 
            {
                Body = messageBody,
                Delay = DelayProvider.CreateRocketDelay(delaySeconds)
            };
            var rocketDispatch = new RocketMessageDispatch { Topic = topic, Tag = tag };

            return new XRocketTask<TMessageBody>
            {
                Params = rocketMessage,
                DelaySeconds = delaySeconds,                
                Dispatch = rocketDispatch
            };
        }
    }
}
