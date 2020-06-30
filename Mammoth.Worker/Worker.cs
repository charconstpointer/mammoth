using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mammoth.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDistributedCache _cache;

        public Worker(ILogger<Worker> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("http://api:5010");
            var client = new Greeter.GreeterClient(channel);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await client.SayHelloAsync(new HelloRequest {Name = "foobar"});
                    _logger.LogInformation("sent!");
                    await _cache.SetStringAsync("Mammoth.Test", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                        token: stoppingToken);
                    _logger.LogInformation("Mammoth.Test set!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}