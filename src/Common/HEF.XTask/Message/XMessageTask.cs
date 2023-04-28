using System;

namespace HEF.XTask
{
    public class XMessageTask<TMessageBody> : XTask<XMessage<TMessageBody>>
    {
        #region Constructor
        public XMessageTask(TMessageBody messageBody)
            : base(null, BuildXMessage(messageBody))
        { }

        public XMessageTask(XMessage<TMessageBody> xMessage)
            : base(null, ValidateXMessage(xMessage))
        { }
        #endregion

        #region Helper Functions
        private static XMessage<TMessageBody> BuildXMessage(TMessageBody messageBody)
        {
            if (messageBody == null)
                throw new ArgumentNullException(nameof(messageBody));

            return new XMessage<TMessageBody>
            {
                Body = messageBody
            };
        }

        private static XMessage<TMessageBody> ValidateXMessage(XMessage<TMessageBody> xMessage)
        {
            if (xMessage == null)
                throw new ArgumentNullException(nameof(xMessage));

            if (xMessage.Body == null)
                throw new ArgumentNullException($"message{nameof(xMessage.Body)}");

            return xMessage;
        }
        #endregion
    }
}
