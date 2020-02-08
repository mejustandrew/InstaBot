using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            try
            {
                var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var preferencesFilePath = Path.Combine(currentDirectory, "Preferences.txt");
                var serializedTags = File.ReadAllText(preferencesFilePath);
                var tags = JsonConvert.DeserializeObject<List<Tag>>(serializedTags);
                await BotRunner.RunBotForTagsAsync(tags, new LoggerWrapper());
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Exception occured when starting the bot. Attempting to stop.");
                throw;
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Service started");
                return base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occured when starting the bot. Attempting to stop.");
                throw;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Service stop triggered");
            try
            {
                BotRunner.StopRunning();
                return base.StartAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Exception occured when stopping the bot.");
                throw;
            }
        }
    }
}
