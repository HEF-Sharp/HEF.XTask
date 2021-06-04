using System;
using System.Threading.Tasks;

namespace HEF.XTask
{
    public class XDelayTask<TParams>
    {
        public XDelayTask(Func<TParams, Task> func, TParams @params, int delaySeconds)
            : this(new XTask<TParams>(func, @params), delaySeconds)
        { }

        public XDelayTask(XTask<TParams> xTask, int delaySeconds)
        {
            if (delaySeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delaySeconds), "delay seconds should not less than zero");

            InnerTask = xTask ?? throw new ArgumentNullException(nameof(xTask));
            DelaySeconds = delaySeconds;
        }

        public XTask<TParams> InnerTask { get; }

        public int DelaySeconds { get; }
    }

    public class XDelayTask<TParams, TResult>
    {
        public XDelayTask(Func<TParams, Task<TResult>> func, TParams @params, int delaySeconds)
            : this(new XTask<TParams, TResult>(func, @params), delaySeconds)
        { }

        public XDelayTask(XTask<TParams, TResult> xTask, int delaySeconds)
        {
            if (delaySeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delaySeconds), "delay seconds should not less than zero");

            InnerTask = xTask ?? throw new ArgumentNullException(nameof(xTask));
            DelaySeconds = delaySeconds;
        }

        public XTask<TParams, TResult> InnerTask { get; }

        public int DelaySeconds { get; }
    }
}
