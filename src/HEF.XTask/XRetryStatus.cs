namespace HEF.XTask
{
    public class XRetryStatus
    {
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; }

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
