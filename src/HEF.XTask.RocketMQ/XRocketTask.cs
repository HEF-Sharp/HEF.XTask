using System;

namespace HEF.XTask.RocketMQ
{
    public class XRocketTask<TMessageBody> : XDelayTask<RocketMessage<TMessageBody>>
    {
        public XRocketTask(TMessageBody messageBody, string topic)
            : this(messageBody, topic, null)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag)
            : this(messageBody, topic, tag, 0)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, int delaySeconds)
            : this(messageBody, topic, null, delaySeconds)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag, int delaySeconds)
            : base(null, BuildRocketMessage(messageBody, topic, tag), delaySeconds)
        { }

        public XRocketTask(RocketMessage<TMessageBody> rocketMessage)
            : this(rocketMessage, 0)
        { }

        public XRocketTask(RocketMessage<TMessageBody> rocketMessage, int delaySeconds)
            : base(null, ValidateRocketMessage(rocketMessage), delaySeconds)
        { }

        private static RocketMessage<TMessageBody> BuildRocketMessage(TMessageBody messageBody,
            string topic, string tag)
        {
            if (messageBody == null)
                throw new ArgumentNullException(nameof(messageBody));

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException(nameof(topic));

            return new RocketMessage<TMessageBody>
            {
                Body = messageBody,
                Dispatch = new RocketDispatch { Topic = topic, Tag = tag }
            };
        }

        private static RocketMessage<TMessageBody> ValidateRocketMessage(RocketMessage<TMessageBody> rocketMessage)
        {
            if (rocketMessage == null)
                throw new ArgumentNullException(nameof(rocketMessage));

            if (rocketMessage.Body == null)
                throw new ArgumentNullException($"message{nameof(rocketMessage.Body)}");

            if (string.IsNullOrWhiteSpace(rocketMessage.Dispatch?.Topic))
                throw new ArgumentNullException(nameof(rocketMessage.Dispatch.Topic));

            return rocketMessage;
        }
     }
}
