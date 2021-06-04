using System;

namespace HEF.XTask.RocketMQ
{
    public class XRocketTask<TMessageBody> : XDelayTask<RocketMessage<TMessageBody>>
    {
        #region Constructor
        public XRocketTask(TMessageBody messageBody, string topic)
            : this(messageBody, topic, null)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag)
            : this(messageBody, topic, tag, 0)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, int retryCount)
            : this(messageBody, topic, null, retryCount)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag, int retryCount)
            : this(messageBody, topic, tag, retryCount, 0)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, int retryCount, int delaySeconds)
            : this(messageBody, topic, null, retryCount, delaySeconds)
        { }

        public XRocketTask(TMessageBody messageBody, string topic, string tag, int retryCount, int delaySeconds)
            : base(null, BuildRocketMessage(messageBody, topic, tag, retryCount), delaySeconds)
        { }

        #region RocketMessage
        public XRocketTask(RocketMessage<TMessageBody> rocketMessage)
            : this(rocketMessage, 0)
        { }

        public XRocketTask(RocketMessage<TMessageBody> rocketMessage, int delaySeconds)
            : base(null, ValidateRocketMessage(rocketMessage), delaySeconds)
        { }
        #endregion

        #endregion

        #region Helper Functions
        private static RocketMessage<TMessageBody> BuildRocketMessage(TMessageBody messageBody,
            string topic, string tag, int retryCount)
        {
            if (messageBody == null)
                throw new ArgumentNullException(nameof(messageBody));

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentNullException(nameof(topic));

            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should not less than zero");

            return new RocketMessage<TMessageBody>
            {
                Body = messageBody,
                Dispatch = new RocketDispatch { Topic = topic, Tag = tag },
                Retry = new XRetryStatus { MaxRetryCount = retryCount }
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

            if (rocketMessage.Retry == null)
                throw new ArgumentNullException(nameof(rocketMessage.Retry));

            if (rocketMessage.Retry.MaxRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(rocketMessage.Retry.MaxRetryCount),
                    "max retry count should not less than zero");

            return rocketMessage;
        }
        #endregion
    }
}
