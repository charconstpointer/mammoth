using System.Threading.Tasks;
using Grpc.Core;
using Mammoth.Api.DTO;
using Mammoth.Api.Grpc;
using Mammoth.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Mammoth.Api.Services
{
    public class PlaylistService : Playlist.PlaylistBase
    {
        private readonly ILogger<PlaylistService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IHubContext<MammothHub> _hubContext;

        public PlaylistService(ILogger<PlaylistService> logger, IDistributedCache cache,
            IHubContext<MammothHub> hubContext)
        {
            _logger = logger;
            _cache = cache;
            _hubContext = hubContext;
        }

        public override async Task<CurrentTrackResponse> Notify(CurrentTrackRequest request, ServerCallContext context)
        {
            var channelId = request.ChannelId;
            _logger.LogInformation($"Received request for track change on channel {channelId}");
            var key = $"CurrentTrack-{channelId}";
            var track = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(track))
            {
                _logger.LogInformation($"Propagating track change to all clients");
                await _hubContext.Clients.All.SendAsync("currentlyPlayedChanged", new PlayedTrackMessage
                {
                    ScheduleId = channelId,
                    Payload = track
                });
            }

            return new CurrentTrackResponse();
        }
    }
}