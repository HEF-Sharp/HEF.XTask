using HEF.MQ.Bus;
using NewLife.RocketMQ.Protocol;
using System;
using System.Threading.Tasks;

namespace HEF.XTask.RocketMQ
{
    public abstract class RocketTaskMessageConsumer<TMessageBody> : IMQTypedMessageConsumer<MessageExt, RocketMessage<TMessageBody>>
    {
        protected RocketTaskMessageConsumer(IRocketTaskScheduler rocketTaskScheduler)
        {
            RocketTaskScheduler = rocketTaskScheduler ?? throw new ArgumentNullException(nameof(rocketTaskScheduler));
        }

        protected IRocketTaskScheduler RocketTaskScheduler { get; }

        public async Task<bool> Consume(MQTypedMessage<MessageExt, RocketMessage<TMessageBody>> typedMessage)
        {
            var rocketMessage = typedMessage.Content;

            if (rocketMessage.Context.DelayStatus.RemainDelaySeconds > 0)
            {
                //还没到延迟时间 通过剩余延迟秒数 发布新的延迟任务
                var nextDelayTask = new XRocketTask<TMessageBody>(rocketMessage);
                return RocketTaskScheduler.Schedule(nextDelayTask, rocketMessage.Context);
            }

            var result = await Consume(typedMessage.Message, rocketMessage);

            var scheduleContext = rocketMessage.Context.ScheduleContext;

            scheduleContext.TimingOnce(); //累加定时次数
            scheduleContext.RetryOnce(); //累加重试次数

            if (scheduleContext.CheckStartTiming())  //定时调度 直接发布下一次执行任务
            {
                var nextIntervalTask = new XRocketTask<TMessageBody>(rocketMessage);
                RocketTaskScheduler.Schedule(nextIntervalTask, rocketMessage.Context);

                return true;
            }

            if (result)  //延迟任务执行成功，无需其它操作
                return true;

            if (!scheduleContext.IsRetrying())  //首次执行失败 通知任务执行失败
            {
                await OnConsumeFailed(typedMessage.Message, rocketMessage);
            }

            if (scheduleContext.CheckStartRetry())
            {
                if (scheduleContext.IsRetryEnd())  //重试结束后依然执行失败 通知任务重试失败
                {
                    await OnRetryFailed(typedMessage.Message, rocketMessage);
                }
                else
                {
                    //任务执行失败 进行重试
                    var retryTask = new XRocketTask<TMessageBody>(rocketMessage);
                    RocketTaskScheduler.Schedule(retryTask, rocketMessage.Context);
                }
            }

            return true;
        }

        protected abstract Task<bool> Consume(MessageExt messageExt, RocketMessage<TMessageBody> rocketMessage);

        protected abstract Task OnConsumeFailed(MessageExt messageExt, RocketMessage<TMessageBody> rocketMessage);

        protected abstract Task OnRetryFailed(MessageExt messageExt, RocketMessage<TMessageBody> rocketMessage);
    }
}
