using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Mammoth.Worker.DTO;
using Mammoth.Worker.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            var httpClient = new HttpClient();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await client.SayHelloAsync(new HelloRequest {Name = "foobar"});
                    _logger.LogInformation("sent!");
                    var id = 1;
                    var day = DateTime.Now;
                    var response = await httpClient.GetStringAsync(
                        $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
                    _logger.LogInformation("Fetched schedule");
                    var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response);
                    await SetCacheEntry(stoppingToken, schedule, id);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task SetCacheEntry(CancellationToken stoppingToken, ScheduleResponse schedule, int id)
        {
            var programs = schedule.Schedule.AsDto();
            var key = $"{DateTime.Now.Date}-{id}";
            var programsSerialized = JsonConvert.SerializeObject(programs);
            await _cache.SetStringAsync(key, programsSerialized, token: stoppingToken);
            _logger.LogInformation($"Schedule cache {key}");
        }
    }
}