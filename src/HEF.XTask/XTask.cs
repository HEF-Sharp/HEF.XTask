using System;
using System.Threading.Tasks;

namespace HEF.XTask
{
    public class XTask<TParams>
    {
        public XTask(Func<TParams, Task> func, TParams @params)
        {
            Function = func;
            Params = @params ?? throw new ArgumentNullException(nameof(@params));            
        }

        public TParams Params { get; }

        public Func<TParams, Task> Function { get; }
    }

    public class XTask<TParams, TResult>
    {
        public XTask(Func<TParams, Task<TResult>> func, TParams @params)
        {
            Function = func;
            Params = @params ?? throw new ArgumentNullException(nameof(@params));
        }

        public TParams Params { get; }

        public Func<TParams, Task<TResult>> Function { get; }
    }
}
