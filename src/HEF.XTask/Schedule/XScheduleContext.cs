namespace HEF.XTask
{
    public class XScheduleContext
    {
        public XScheduleOptions ScheduleOptions { get; set; }

        public XRetryStatus RetryStatus { get; set; } = new XRetryStatus();

        #region DelayMethods
        public int GetDelaySeconds()
        {
            return ScheduleOptions.Type switch
            {
                XScheduleType.Delay => ScheduleOptions.DelaySeconds,
                XScheduleType.Timing => ScheduleOptions.IntervalSeconds,
                _ => 0
            };
        }
        #endregion

        #region RetryMethods
        public bool StartRetry()
        {
            if (RetryStatus.IsRetrying)
                return false;

            if (ScheduleOptions.MaxRetryCount < 1)
                return false;

            RetryStatus.IsRetrying = true;
            RetryStatus.RetriedCount = 0;

            return true;
        }

        public bool IsRetryEnd()
        {
            return RetryStatus.IsRetrying
                && RetryStatus.RetriedCount >= ScheduleOptions.MaxRetryCount;
        }

        public void RetryOnce()
        {
            if (RetryStatus.IsRetrying
                && RetryStatus.RetriedCount < ScheduleOptions.MaxRetryCount)
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
}
