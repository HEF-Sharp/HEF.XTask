namespace HEF.XTask.RocketMQ
{
    public class XRocketTask<TMessageBody> : XDelayTask<RocketMessage<TMessageBody>>
    { 
        public RocketMessageDispatch Dispatch { get; set; }
    }

    /// <summary>
    /// Rocket消息分发
    /// </summary>
    public class RocketMessageDispatch
    {
        public string Topic { get; set; }

        public string Tag { get; set; }
    }
}
