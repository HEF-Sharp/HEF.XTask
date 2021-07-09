using System;

namespace HEF.XTask
{
    public interface IXTaskScheduler<TParams>
    {
        bool Schedule(XTask<TParams> task, XScheduleContext scheduleContext);
    }

    public interface IXTaskScheduler<TParams, TResult>
    {
        bool Schedule(XTask<TParams, TResult> task, XScheduleContext scheduleContext);
    }
}
