using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mammoth.Worker
{
    public class App
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    if (hostContext.HostingEnvironment.IsProduction())
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = "redis";
                            options.InstanceName = "Mammoth";
                        });
                    else
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = "localhost";
                            options.InstanceName = "Mammoth";
                        });
                    services.AddHostedService<PlaylistWorker>();
                });
        }
    }
}