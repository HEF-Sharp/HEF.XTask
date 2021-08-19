using System;

namespace HEF.XTask
{
    public static class XScheduleContextFactory
    {
        public static XScheduleContext Instant(int retryCount)
        {
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should not less than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Instant, MaxRetryCount = retryCount };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext Delay(TimeSpan delay, int retryCount)
        {
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should not less than zero");

            var delaySeconds = Convert.ToInt32(Math.Floor(delay.TotalSeconds));
            if (delaySeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(delay), "delay seconds should greater than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Delay, DelaySeconds = delaySeconds, MaxRetryCount = retryCount };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext Timing(TimeSpan interval, TimeSpan timeout = default)
        {
            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing interval seconds should greater than zero");

            var timeoutSeconds = Convert.ToInt32(Math.Floor(timeout.TotalSeconds));
            if (timeoutSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing timeout seconds should not less than zero");

            if (timeoutSeconds > 0 && timeoutSeconds < intervalSeconds)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing timeout seconds should not less than interval seconds");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Timing, IntervalSeconds = intervalSeconds, TimeoutSeconds = timeoutSeconds };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext Retry(int retryCount)
        {
            if (retryCount < 1)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should greater than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Retry, MaxRetryCount = retryCount };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext TimingRetry(TimeSpan interval, TimeSpan timeout)
        {
            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing retry interval seconds should greater than zero");

            var timeoutSeconds = Convert.ToInt32(Math.Floor(timeout.TotalSeconds));
            if (timeoutSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing retry timeout seconds should greater than zero");

            if (timeoutSeconds < intervalSeconds)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing retry timeout seconds should not less than interval seconds");

            //定时重试也隶属于重试调度（不走定时调度），通过Timeout时间计算出最大重试次数
            var maxRetryCount = Convert.ToInt32(Math.Floor(timeoutSeconds / (double)intervalSeconds));

            var scheduleOptions = new XScheduleOptions
            {
                Type = XScheduleType.TimingRetry,
                IntervalSeconds = intervalSeconds,
                TimeoutSeconds = timeoutSeconds,
                MaxRetryCount = maxRetryCount
            };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext DelayTiming(TimeSpan delay, TimeSpan interval, TimeSpan timeout = default)
        {
            var delaySeconds = Convert.ToInt32(Math.Floor(delay.TotalSeconds));
            if (delaySeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(delay), "delay seconds should greater than zero");

            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing interval seconds should greater than zero");

            var timeoutSeconds = Convert.ToInt32(Math.Floor(timeout.TotalSeconds));
            if (timeoutSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing timeout seconds should not less than zero");

            if (timeoutSeconds > 0 && timeoutSeconds < intervalSeconds)
                throw new ArgumentOutOfRangeException(nameof(timeout), "timing timeout seconds should not less than interval seconds");

            var scheduleOptions = new XScheduleOptions
            {
                Type = XScheduleType.DelayTiming,
                DelaySeconds = delaySeconds,
                IntervalSeconds = intervalSeconds,
                TimeoutSeconds = timeoutSeconds
            };

            return scheduleOptions.BuildScheduleContext();
        }

        internal static XScheduleContext BuildScheduleContext(this XScheduleOptions scheduleOptions)
        {
            if (scheduleOptions == null)
                throw new ArgumentNullException(nameof(scheduleOptions));

            return new XScheduleContext { ScheduleOptions = scheduleOptions };
        }
    }
}
