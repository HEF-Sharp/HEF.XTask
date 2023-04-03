namespace HEF.XTask.Queue
{
    public class RedisQueueConfig
    {
        /// <summary>
        /// 列表Key
        /// </summary>
        public string ListKey { get; set; }

        /// <summary>
        /// 待确认列表Key
        /// </summary>
        public string ConfirmListKey { get; set; }
    }
}
