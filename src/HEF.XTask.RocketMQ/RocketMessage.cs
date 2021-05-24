namespace HEF.XTask.RocketMQ
{
    /// <summary>
    /// Rocket消息
    /// </summary>    
    public class RocketMessage<TMessageBody>
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public TMessageBody Body { get; set; }

        /// <summary>
        /// 延迟信息
        /// </summary>
        public RocketDelay Delay { get; set; }
    }

    public class RocketDelay
    {
        /// <summary>
        /// 延迟时间等级
        /// </summary>
        public int DelayTimeLevel { get; set; }

        /// <summary>
        /// 剩余延迟秒数
        /// </summary>
        public int RemainDelaySeconds { get; set; }
    }
}
