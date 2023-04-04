using Microsoft.Extensions.DependencyInjection;

namespace HEF.XTask.Hosting
{
    public interface IXTaskHostingConfigurator
    {
        IServiceCollection Services { get; }
    }
}
