using System;
using System.Threading.Tasks;

namespace HEF.XTask.Runner
{
    public interface IXTaskExceptionHandler<TParam>
    {
        Task HandleExceptionTaskAsync(XTask<TParam> exceptionTask, Exception exception);
    }
}
