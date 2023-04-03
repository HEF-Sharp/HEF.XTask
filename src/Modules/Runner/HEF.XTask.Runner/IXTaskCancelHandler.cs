using System.Threading.Tasks;

namespace HEF.XTask.Runner
{
    public interface IXTaskCancelHandler<TParam>
    {
        Task HandleCancelledTaskAsync(XTask<TParam> cancelledTask);
    }
}
