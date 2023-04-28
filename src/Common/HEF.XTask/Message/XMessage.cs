namespace HEF.XTask
{
    public class XMessage<TMessageBody>
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public TMessageBody Body { get; set; }

        /// <summary>
        /// Context
        /// </summary>
        public XScheduleContext Context { get; set; }
    }
}
