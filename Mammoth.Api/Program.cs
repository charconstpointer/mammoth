using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace Mammoth.Api
{
    public class App
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(5010,
                            listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                        options.ListenAnyIP(5011,
                            listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
                    });
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}