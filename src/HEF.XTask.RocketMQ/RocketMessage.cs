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

        /// <summary>
        /// 分发信息
        /// </summary>
        public RocketDispatch Dispatch { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 0;
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

    /// <summary>
    /// Rocket分发
    /// </summary>
    public class RocketDispatch
    {
        public string Topic { get; set; }

        public string Tag { get; set; }
    }
}
