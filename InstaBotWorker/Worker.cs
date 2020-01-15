using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InstaBotApi;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace InstaBotWorker
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Runnin started");
            var serializedTags = File.ReadAllText("Preferences.txt");
            var tags = JsonConvert.DeserializeObject<List<Tag>>(serializedTags);
            await BotRunner.RunBotForTagsAsync(tags, new LoggerWrapper()) ;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Service started");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Service stop triggered");
            BotRunner.StopRunning();
            return base.StartAsync(cancellationToken);
        }
    }
}
