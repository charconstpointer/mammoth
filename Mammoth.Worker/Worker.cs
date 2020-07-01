using System;
using System.Collections.Generic;
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
        private readonly IDistributedCache _cache;
        private readonly ILogger<Worker> _logger;
        private readonly Random _random = new Random();
        private readonly ICollection<string> _words = new List<string> {"foo", "bar", "baz", "clazz"};

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
                await ProcessSchedules(stoppingToken, client, httpClient);
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task ProcessSchedules(CancellationToken stoppingToken, Greeter.GreeterClient client,
            HttpClient httpClient)
        {
            try
            {
                await client.SayHelloAsync(new HelloRequest {Name = "foobar"});
                for (var i = 0; i < 10; i++)
                {
                    _logger.LogInformation($"Fetching schedule #id = {i}");
                    await FetchSchedule(i, httpClient, stoppingToken);
                    _logger.LogInformation($"Schedule #id = {i} fetched successfully");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task FetchSchedule(int id, HttpClient httpClient, CancellationToken stoppingToken)
        {
            var day = DateTime.Now;
            var response = await httpClient.GetStringAsync(
                $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
            _logger.LogInformation("Fetched schedule");
            var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response);
            await SetCacheEntry(schedule, stoppingToken, id);
        }

        private async Task SetCacheEntry(ScheduleResponse schedule, CancellationToken stoppingToken, int id)
        {
            var programs = schedule.Schedule.AsDto();
            var key = $"{DateTime.Now.Date}-{id}";
            var programsSerialized = JsonConvert.SerializeObject(programs);
            await _cache.SetStringAsync(key, programsSerialized, stoppingToken);
            _logger.LogInformation($"Schedule cache {key}");
        }
    }
}