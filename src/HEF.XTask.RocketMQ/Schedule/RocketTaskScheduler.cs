using NewLife.RocketMQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketTaskScheduler
    {
        bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext);
    }

    public class RocketTaskScheduler : IRocketTaskScheduler
    {
        public RocketTaskScheduler(IRocketDelayProvider delayProvider,
            IRocketMQProducerProvider producerProvider)
        {
            DelayProvider = delayProvider ?? throw new ArgumentNullException(nameof(delayProvider));
            ProducerProvider = producerProvider ?? throw new ArgumentNullException(nameof(producerProvider));
        }

        protected IRocketDelayProvider DelayProvider { get; }

        protected IRocketMQProducerProvider ProducerProvider { get; }

        public bool Schedule<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext)
        {
            if (rocketTask == null)
                throw new ArgumentNullException(nameof(rocketTask));

            if (rocketTaskContext == null)
                throw new ArgumentNullException(nameof(rocketTaskContext));

            if (rocketTaskContext.DelayStatus.RemainDelaySeconds > 0)
                return ScheduleDelay(rocketTask, rocketTaskContext);

            if (rocketTaskContext.ScheduleContext.IsTiming())
                return ScheduleTiming(rocketTask, rocketTaskContext);

            if (rocketTaskContext.ScheduleContext.IsRetrying())
                return ScheduleRetry(rocketTask, rocketTaskContext);

            return rocketTaskContext switch
            {
                RocketTaskContext { ScheduleContext: { ScheduleOptions: { Type: XScheduleType.Instant } } } => ScheduleInstant(rocketTask, rocketTaskContext),
                RocketTaskContext { ScheduleContext: { ScheduleOptions: { Type: XScheduleType.Delay } } } => ScheduleDelay(rocketTask, rocketTaskContext),
                RocketTaskContext { ScheduleContext: { ScheduleOptions: { Type: XScheduleType.Timing } } } => ScheduleTiming(rocketTask, rocketTaskContext),
                RocketTaskContext { ScheduleContext: { ScheduleOptions: { Type: XScheduleType.Retry } } } => ScheduleRetry(rocketTask, rocketTaskContext),
                RocketTaskContext { ScheduleContext: { ScheduleOptions: { Type: XScheduleType.DelayTiming } } } => ScheduleDelay(rocketTask, rocketTaskContext),
                _ => throw new NotSupportedException("not supported task schedule")
            };
        }

        protected bool ScheduleInstant<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext)
        {
            rocketTaskContext.DelayStatus = DelayProvider.CreateRocketDelay(0);

            rocketTask.Params.Context = rocketTaskContext;
            return PublishRocketMessage(rocketTask.Params);
        }

        protected bool ScheduleDelay<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext)
        {
            var delaySeconds = rocketTaskContext.GetDelaySeconds();
            rocketTaskContext.DelayStatus = DelayProvider.CreateRocketDelay(delaySeconds);

            rocketTask.Params.Context = rocketTaskContext;
            return PublishRocketMessage(rocketTask.Params);
        }

        protected bool ScheduleTiming<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext)
        {
            var scheduleContext = rocketTaskContext.ScheduleContext;

            if (!scheduleContext.CheckStartTiming())
                return false;   //启动定时失败

            var intervalSeconds = scheduleContext.ScheduleOptions.IntervalSeconds;
            rocketTaskContext.DelayStatus = DelayProvider.CreateRocketDelay(intervalSeconds);

            rocketTask.Params.Context = rocketTaskContext;
            return PublishRocketMessage(rocketTask.Params);
        }

        protected bool ScheduleRetry<TMessageBody>(XRocketTask<TMessageBody> rocketTask, RocketTaskContext rocketTaskContext)
        {
            var scheduleContext = rocketTaskContext.ScheduleContext;

            if (scheduleContext.IsRetryEnd())
                return false;  //已达最大重试次数，结束重试

            if (!scheduleContext.CheckStartRetry())
                return false;   //启动重试失败(最大重试次数小于1)，不需要重试

            rocketTaskContext.DelayStatus = GetRetryRocketDelay(rocketTaskContext);            
            
            rocketTask.Params.Context = rocketTaskContext;
            return PublishRocketMessage(rocketTask.Params);
        }

        #region Helper Functions
        private bool PublishRocketMessage<TMessageBody>(RocketMessage<TMessageBody> rocketMessage)
        {
            var typedProducer = ProducerProvider.GetTypedProducer(rocketMessage.Dispatch.Topic);

            var result = typedProducer.Publish(rocketMessage,
                b => b.Tag(rocketMessage.Dispatch.Tag).Delay(rocketMessage.Context.DelayStatus.DelayTimeLevel));

            return result.Status == SendStatus.SendOK;
        }

        private RocketDelayStatus GetRetryRocketDelay(RocketTaskContext rocketTaskContext)
        {
            if (rocketTaskContext.ScheduleContext.RetryStatus.RetriedCount > 0)
                return DelayProvider.GetNextRocketDelay(rocketTaskContext.DelayStatus.DelayTimeLevel);

            return DelayProvider.GetMinRocketDelay();  //首次重试Delay
        }
        #endregion
    }
}
