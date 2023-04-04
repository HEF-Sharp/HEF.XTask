using Microsoft.Extensions.DependencyInjection;

namespace HEF.XTask.Hosting
{
    public class XTaskHostingConfigurator : IXTaskHostingConfigurator
    {
        public XTaskHostingConfigurator(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
