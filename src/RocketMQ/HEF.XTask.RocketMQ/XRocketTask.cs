using System;

namespace HEF.XTask.RocketMQ
{
    public class XRocketTask<TMessageBody> : XTask<RocketMessage<TMessageBody>>
    {
        #region Constructor
        public XRocketTask(TMessageBody messageBody, string topic)
            : this(messageBody, topic, null)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag)
            : base(null, BuildRocketMessage(messageBody, topic, tag))
        { }

        public XRocketTask(RocketMessage<TMessageBody> rocketMessage)
            : base(null, ValidateRocketMessage(rocketMessage))
        { }
        #endregion

        #region Helper Functions
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
        #endregion
    }
}
