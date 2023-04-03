using System.Threading;
using System.Threading.Tasks;

namespace HEF.XTask.Runner
{
    public interface IXTaskExecutor<TParam>
    {
        Task<bool> ExecuteTaskAsync(XTask<TParam> toRunTask, CancellationToken cancellationToken);
    }
}
