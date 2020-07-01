using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mammoth.Core.Entities;
using Mammoth.Worker.DTO;
using Mammoth.Worker.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mammoth.Worker
{
    public class PlaylistWorker : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlaylistWorker> _logger;

        public PlaylistWorker(ILogger<PlaylistWorker> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var playlist = new Playlist();
                var id = 1;
                var day = DateTime.Now;
                var response = await _httpClient.GetStringAsync(
                    $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
                _logger.LogInformation("Fetched schedule");
                var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response)
                    .Schedule.OrderBy(s=>s.StopHour)
                    .AsDto();
                playlist.AddChannel(id, schedule.AsEntity());
                playlist.TrackChanged += (sender, change) =>
                    _logger.LogInformation($"{change.Track.Title} started playing on {change.ChannelId}");
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Currently playing {playlist.GetCurrentlyPlayed(1).Description}");
                    await Task.Delay(999999999, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}