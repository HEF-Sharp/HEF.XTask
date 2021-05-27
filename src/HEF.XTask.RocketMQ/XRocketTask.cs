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
    }
}
