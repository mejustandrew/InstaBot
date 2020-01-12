using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;

namespace InstaBotWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).UseWindowsService().Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureLogging(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });

        private static void ConfigureLogging(IConfiguration configuration)
        {
            var loggingPath = configuration.GetValue<string>("Logging:OutputPath");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(loggingPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
