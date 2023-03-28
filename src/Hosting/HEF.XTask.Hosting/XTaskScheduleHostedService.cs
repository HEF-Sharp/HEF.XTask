using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.XTask.Hosting
{
    public class XTaskScheduleHostedService : BackgroundService
    {
        public XTaskScheduleHostedService(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
        }

        protected IServiceScopeFactory ScopeFactory { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            await ProcessingStartToRunTaskAsync(stoppingToken);
        }

        private async Task ProcessingStartToRunTaskAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

            }

            throw new NotImplementedException();
        }
    }
}
