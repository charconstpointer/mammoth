using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    if (hostContext.HostingEnvironment.IsProduction())
                    {
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = "redis";
                            options.InstanceName = "Mammoth";
                        });    
                    }
                    else
                    {
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = "localhost";
                            options.InstanceName = "Mammoth";
                        });
                    }
                    
                    // services.AddHostedService<Worker>();
                    services.AddHostedService<ScheduleWatcher>();
                });
    }
}