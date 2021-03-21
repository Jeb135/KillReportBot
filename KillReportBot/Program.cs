using KillReportBot.KillReports;
using KillReportBot.WHCoin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KillReportBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(ServiceConfiguration.Name));
                    //services.AddHostedService<KillReportWorker>();
                    services.AddHostedService<WHCoinWorker>();
                });
        }
    }
}
