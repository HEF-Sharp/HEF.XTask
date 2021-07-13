namespace HEF.XTask.RocketMQ
{
    /// <summary>
    /// Rocket任务Context
    /// </summary>
    public class RocketTaskContext
    {
        /// <summary>
        /// 调度Context
        /// </summary>
        public XScheduleContext ScheduleContext { get; set; }

        /// <summary>
        /// 延迟状态
        /// </summary>
        public RocketDelayStatus DelayStatus { get; set; } = new RocketDelayStatus();

        #region Methods
        public int GetDelaySeconds()
        {
            if (DelayStatus.RemainDelaySeconds > 0)
                return DelayStatus.RemainDelaySeconds;

            return ScheduleContext.ScheduleOptions.DelaySeconds;
        }

        public RocketTaskContext ResetRocketDelay()
        {
            DelayStatus = new RocketDelayStatus();

            return this;
        }
        #endregion
    }

    public class RocketDelayStatus
    {
        /// <summary>
        /// 延迟时间等级
        /// </summary>
        public int DelayTimeLevel { get; set; }

        /// <summary>
        /// 剩余延迟秒数
        /// </summary>
        public int RemainDelaySeconds { get; set; }
    }
}
