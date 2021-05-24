using System;
using System.Threading.Tasks;

namespace HEF.XTask
{
    public class XTask<TParams>
    {
        public TParams Params { get; set; }

        public Func<TParams, Task> Function { get; set; }
    }
}
