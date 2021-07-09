using System;

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

    public class XScheduleOptionsFactory
    {
        public static XScheduleOptions Instant(int retryCount)
        {
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should not less than zero");

            return new XScheduleOptions { Type = XScheduleType.Instant, MaxRetryCount = retryCount };
        }

        public static XScheduleOptions Delay(TimeSpan delay, int retryCount)
        {
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should not less than zero");

            var delaySeconds = Convert.ToInt32(Math.Floor(delay.TotalSeconds));
            if (delaySeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(delay), "delay seconds should greater than zero");

            return new XScheduleOptions { Type = XScheduleType.Delay, DelaySeconds = delaySeconds, MaxRetryCount = retryCount };
        }

        public static XScheduleOptions Timing(TimeSpan interval)
        {
            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing interval seconds should greater than zero");

            return new XScheduleOptions { Type = XScheduleType.Timing, IntervalSeconds = intervalSeconds };
        }

        public static XScheduleOptions Retry(int retryCount)
        {
            if (retryCount < 1)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should greater than zero");

            return new XScheduleOptions { Type = XScheduleType.Retry, MaxRetryCount = retryCount };
        }
    }
}
