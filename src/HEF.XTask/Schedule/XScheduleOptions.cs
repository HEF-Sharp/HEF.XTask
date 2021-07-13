namespace HEF.XTask
{
    public enum XScheduleType
    {
        /// <summary>
        /// 立即执行
        /// </summary>
        Instant,
        /// <summary>
        /// 延迟执行
        /// </summary>
        Delay,
        /// <summary>
        /// 定时执行
        /// </summary>
        Timing,
        /// <summary>
        /// 重试执行
        /// </summary>
        Retry
    }

    /// <summary>
    /// 调度配置
    /// </summary>
    public class XScheduleOptions
    {
        /// <summary>
        /// 调度类型
        /// </summary>
        public XScheduleType Type { get; set; }

        /// <summary>
        /// 延迟秒数
        /// </summary>
        public int DelaySeconds { get; set; }

        /// <summary>
        /// 间隔秒数
        /// </summary>
        public int IntervalSeconds { get; set; }

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; }
    }
}
