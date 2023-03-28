using System;

namespace HEF.XTask.RocketMQ
{
    public static class RocketTaskContextExtensions
    {
        public static RocketTaskContext BuildRocketTaskContext(this XScheduleContext scheduleContext)
        {
            if (scheduleContext == null)
                throw new ArgumentNullException(nameof(scheduleContext));

            return new RocketTaskContext { ScheduleContext = scheduleContext };
        }
    }
}
