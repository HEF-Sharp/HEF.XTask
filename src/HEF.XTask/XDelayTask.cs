using System;
using System.Threading.Tasks;

namespace HEF.XTask
{
    public class XDelayTask<TParams> : XTask<TParams>
    {
        public XDelayTask(Func<TParams, Task> func, TParams @params, int delaySeconds)
            : base(func, @params)
        {
            if (delaySeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delaySeconds), "delay seconds should not less than zero");

            DelaySeconds = delaySeconds;
        }

        public int DelaySeconds { get; }
    }

    public class XDelayTask<TParams, TResult> : XTask<TParams, TResult>
    {
        public XDelayTask(Func<TParams, Task<TResult>> func, TParams @params, int delaySeconds)
            : base(func, @params)
        {
            if (delaySeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(delaySeconds), "delay seconds should not less than zero");

            DelaySeconds = delaySeconds;
        }

        public int DelaySeconds { get; }
    }
}
