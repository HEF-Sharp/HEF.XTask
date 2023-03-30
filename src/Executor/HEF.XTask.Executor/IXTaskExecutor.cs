using System.Threading;
using System.Threading.Tasks;

namespace HEF.XTask.Executor
{
    public interface IXTaskExecutor<TParam>
    {
        Task<bool> ExecuteTaskAsync(XTask<TParam> toRunTask, CancellationToken cancellationToken);
    }
}
