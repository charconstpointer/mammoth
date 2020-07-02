using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Mammoth.Worker.DTO;
using Mammoth.Worker.Extensions;
using Mammoth.Worker.Grpc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Playlist = Mammoth.Core.Entities.Playlist;

namespace Mammoth.Worker
{
    public class PlaylistWorker : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly ILogger<PlaylistWorker> _logger;

        public PlaylistWorker(ILogger<PlaylistWorker> logger, IDistributedCache cache)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                var channel = GrpcChannel.ForAddress("http://api:5010");
                var client = new Grpc.Playlist.PlaylistClient(channel);
                var playlist = new Playlist();
                await SetupPlaylist(playlist, client);
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(999999999, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task SetupPlaylist(Playlist playlist, Grpc.Playlist.PlaylistClient client)
        {
            var ids = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
            foreach (var id in ids)
            {
                var schedule = await FetchSchedule(id);
                playlist.AddChannel(id, schedule.AsEntity());
            }

            playlist.TrackChanged += async (sender, change) =>
            {
                var key = $"CurrentTrack-{change.ChannelId}";
                _logger.LogInformation(
                    $"{change.ChannelId} started playing #{change.Track.Id} => {change.Track.Title} / {change.Track.Description}");
                await _cache.SetStringAsync(key, JsonConvert.SerializeObject(change.Track));
                _logger.LogInformation($"Settings cache entry {key}");
                await client.NotifyAsync(new CurrentTrackRequest
                {
                    ChannelId = change.ChannelId
                });
            };
        }

        private async Task<IEnumerable<ProgramDto>> FetchSchedule(int id)
        {
            var day = DateTime.Now;
            var response = await _httpClient.GetStringAsync(
                $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
            _logger.LogInformation("Fetched schedule");
            var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response)
                .Schedule.OrderBy(s => s.StopHour)
                .AsDto();
            return schedule;
        }
    }
}