﻿namespace HEF.XTask.RocketMQ
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
        /// 分发信息
        /// </summary>
        public RocketDispatch Dispatch { get; set; }

        /// <summary>
        /// Context
        /// </summary>
        public RocketTaskContext Context { get; set; }
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
