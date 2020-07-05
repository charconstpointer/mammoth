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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Playlist = Mammoth.Core.Entities.Playlist;

namespace Mammoth.Worker
{
    public class PlaylistWorker : BackgroundService
    {
        private readonly IDistributedCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlaylistWorker> _logger;
        private readonly string _connectionString;

        public PlaylistWorker(ILogger<PlaylistWorker> logger, IDistributedCache cache, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _cache = cache;
            _connectionString = configuration.GetConnectionString("Api");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new Grpc.Playlist.PlaylistClient(channel);
            var playlist = new Playlist();
            var fetchDate = DateTime.UtcNow.AddHours(2); //init date
            SetupPlaylist(playlist, client);
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.UtcNow.AddHours(2); //time in pl
                if (currentTime != fetchDate)
                {
                    //Day has changed, fetch new schedule etc.
                    SetupPlaylist(playlist, client);
                }

                //tracks are changed not more often than every minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void SetupPlaylist(Playlist playlist, Grpc.Playlist.PlaylistClient client)
        {
            try
            {
                var ids = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
                Parallel.ForEach(ids, async id =>
                {
                    var schedule = await FetchSchedule(id);
                    _logger.LogInformation($"Fetching schedule#{id}");
                    playlist.AddChannel(id, schedule.AsEntity());
                });
                playlist.TrackChanged += async (sender, change) =>
                {
                    var key = $"CurrentTrack-{change.ChannelId}";
                    _logger.LogInformation(
                        $"{change.ChannelId} started playing #{change.Track.Id} => {change.Track.StartHour} - {change.Track.StopHour} {change.Track.Title} / {change.Track.Description}");
                    await _cache.SetStringAsync(key, JsonConvert.SerializeObject(change.Track));
                    _logger.LogInformation($"Settings cache entry {key}");
                    await client.NotifyAsync(new CurrentTrackRequest
                    {
                        ChannelId = change.ChannelId
                    });
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.StackTrace}");
            }
        }

        private async Task<IEnumerable<ProgramDto>> FetchSchedule(int id)
        {
            var day = DateTime.UtcNow.AddHours(2);
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