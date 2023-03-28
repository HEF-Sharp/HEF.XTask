namespace HEF.XTask
{
    /// <summary>
    /// 调度Context
    /// </summary>
    public class XScheduleContext
    {
        /// <summary>
        /// 调度配置
        /// </summary>
        public XScheduleOptions ScheduleOptions { get; set; }

        /// <summary>
        /// 重试状态
        /// </summary>
        public XRetryStatus RetryStatus { get; set; } = new XRetryStatus();

        /// <summary>
        /// 定时状态
        /// </summary>
        public XTimingStatus TimingStatus { get; set; } = new XTimingStatus();

        #region TimingMethods
        public bool IsTimingSchedule()
        {
            return ScheduleOptions.Type == XScheduleType.Timing
                || ScheduleOptions.Type == XScheduleType.DelayTiming;
        }

        public bool CheckStartTiming()
        {
            if (TimingStatus.IsTiming)
                return true;

            if (!IsTimingSchedule())
                return false;

            TimingStatus.IsTiming = true;
            TimingStatus.TimedCount = 0;

            return true;
        }

        public bool IsTiming() => TimingStatus.IsTiming;

        public bool IsTimingTimeout()
        {
            return TimingStatus.IsTiming
                && ScheduleOptions.TimeoutSeconds > 0
                && (TimingStatus.TimedCount + 1) * ScheduleOptions.IntervalSeconds > ScheduleOptions.TimeoutSeconds;
        }

        public void TimingOnce()
        {
            if (IsTiming() && !IsTimingTimeout())
            {
                TimingStatus.TimedCount++;
            }
        }
        #endregion

        #region RetryMethods
        public bool CheckStartRetry()
        {
            if (RetryStatus.IsRetrying)
                return true;

            if (ScheduleOptions.MaxRetryCount < 1)
                return false;

            RetryStatus.IsRetrying = true;
            RetryStatus.RetriedCount = 0;

            return true;
        }

        public bool IsRetrying() => RetryStatus.IsRetrying;

        public bool IsRetryEnd()
        {
            return RetryStatus.IsRetrying
                && RetryStatus.RetriedCount >= ScheduleOptions.MaxRetryCount;
        }

        public void RetryOnce()
        {
            if (IsRetrying() && !IsRetryEnd())
            {
                RetryStatus.RetriedCount++;
            }
        }
        #endregion
    }

    public class XRetryStatus
    {
        /// <summary>
        /// 已重试次数
        /// </summary>
        public int RetriedCount { get; set; }

        /// <summary>
        /// 是否正在重试
        /// </summary>
        public bool IsRetrying { get; set; }
    }

    public class XTimingStatus
    {
        /// <summary>
        /// 已定时执行次数
        /// </summary>
        public long TimedCount { get; set; }

        /// <summary>
        /// 是否正在定时
        /// </summary>
        public bool IsTiming { get; set; }
    }
}
