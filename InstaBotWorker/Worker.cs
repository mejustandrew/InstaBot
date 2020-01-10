using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InstaBotWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;//TODO: implement and register ILogger

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.Write(true);
            _logger.LogInformation("started execute");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("finished execution");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("start async");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("stop async");

            return base.StopAsync(cancellationToken);
        }
    }
}
