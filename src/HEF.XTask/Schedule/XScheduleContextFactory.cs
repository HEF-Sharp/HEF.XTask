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

        public static XScheduleContext Timing(TimeSpan interval)
        {
            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing interval seconds should greater than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Timing, IntervalSeconds = intervalSeconds };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext Retry(int retryCount)
        {
            if (retryCount < 1)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "retry count should greater than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.Retry, MaxRetryCount = retryCount };

            return scheduleOptions.BuildScheduleContext();
        }

        public static XScheduleContext DelayTiming(TimeSpan delay, TimeSpan interval)
        {
            var delaySeconds = Convert.ToInt32(Math.Floor(delay.TotalSeconds));
            if (delaySeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(delay), "delay seconds should greater than zero");

            var intervalSeconds = Convert.ToInt32(Math.Floor(interval.TotalSeconds));
            if (intervalSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "timing interval seconds should greater than zero");

            var scheduleOptions = new XScheduleOptions { Type = XScheduleType.DelayTiming, DelaySeconds = delaySeconds, IntervalSeconds = intervalSeconds };

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
