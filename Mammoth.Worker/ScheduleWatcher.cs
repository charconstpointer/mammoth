using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Mammoth.Worker.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mammoth.Worker
{
    public class ScheduleWatcher : BackgroundService
    {
        private readonly IDistributedCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScheduleWatcher> _logger;
        private Program _currentlyPlayed;

        public ScheduleWatcher(ILogger<ScheduleWatcher> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
            _httpClient = new HttpClient();
            _currentlyPlayed = null;
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
                    await CheckPlayedTrack(client);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.StackTrace);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckPlayedTrack(Greeter.GreeterClient client)
        {
            _logger.LogInformation("spinning");
            var day = DateTime.Now;
            var id = 1;
            var response = await _httpClient.GetStringAsync(
                $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
            _logger.LogInformation("Fetched schedule");
            var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response);
            var currentlyPlayed = schedule.Schedule.Single(p => p.IsActive);
            if (_currentlyPlayed is null)
            {
                _logger.LogInformation($"Track is empty, initializing with {currentlyPlayed.Id}");
                await UpdateCurrentlyPlayed(currentlyPlayed, client);
            }
            else
            {
                if (_currentlyPlayed.Id != currentlyPlayed.Id)
                {
                    _logger.LogInformation($"Track has changed, replacing with {currentlyPlayed.Id}");
                    await UpdateCurrentlyPlayed(currentlyPlayed, client);
                }
                else
                {
                    _logger.LogInformation("Track has not changed, skipping");
                }
            }
        }

        private async Task UpdateCurrentlyPlayed(Program currentlyPlayed, Greeter.GreeterClient client)
        {
            var id = 1;
            _currentlyPlayed = currentlyPlayed;
            var key = $"Currently-Played-{id}";
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(currentlyPlayed));
            _logger.LogInformation($"{key} --- {currentlyPlayed}");
            await client.SayHelloAsync(new HelloRequest {Name = "."});
            _logger.LogInformation("Notified about currently played track");
        }
    }
}