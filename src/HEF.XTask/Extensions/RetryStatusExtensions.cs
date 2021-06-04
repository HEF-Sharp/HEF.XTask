using System;

namespace HEF.XTask
{
    public static class RetryStatusExtensions
    {
        public static bool IsRetryEnd(this XRetryStatus retryStatus)
        {
            if (retryStatus == null)
                throw new ArgumentNullException(nameof(retryStatus));

            return retryStatus.IsRetrying
                && retryStatus.RetriedCount >= retryStatus.MaxRetryCount;
        }

        public static void RetryOnce(this XRetryStatus retryStatus)
        {
            if (retryStatus == null)
                throw new ArgumentNullException(nameof(retryStatus));

            if (retryStatus.IsRetrying && !retryStatus.IsRetryEnd())
            {
                retryStatus.RetriedCount++;
            }
        }

        public static bool StartRetry(this XRetryStatus retryStatus)
        {
            if (retryStatus == null)
                throw new ArgumentNullException(nameof(retryStatus));

            if (retryStatus.IsRetrying)
                return false;

            if (retryStatus.MaxRetryCount < 1)
                return false;

            retryStatus.IsRetrying = true;
            retryStatus.RetriedCount = 0;

            return true;
        }
    }
}
