using System.Threading.Tasks;
using Grpc.Core;
using Mammoth.Api.DTO;
using Mammoth.Api.Hubs;
using Mammoth.Schedule.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Mammoth.Api.Services
{
    public class ScheduleService : Schedule.Server.Schedule.ScheduleBase
    {
        private readonly IHubContext<MammothHub> _hubContext;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IHubContext<MammothHub> hubContext, IDistributedCache cache,
            ILogger<ScheduleService> logger)
        {
            _hubContext = hubContext;
            _cache = cache;
            _logger = logger;
        }

        public override async Task<CurrentTrackResponse> Notify(CurrentTrackRequest request, ServerCallContext context)
        {
            var channelId = request.ChannelId;
            var key = $"CurrentTrack-{channelId}";
            _logger.LogInformation($"Reading from cache #{key}");
            var currentTrack = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(currentTrack))
            {
                await _hubContext.Clients.All.SendAsync("currentlyPlayedChanged", new PlayedTrackMessage
                {
                    ScheduleId = channelId,
                    Payload = currentTrack
                });
            }

            return new CurrentTrackResponse();
        }
    }
}