using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Mammoth.Api.DTO;
using Mammoth.Api.Hubs;
using Mammoth.Worker;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Mammoth.Api.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly IDistributedCache _cache;
        private readonly ICollection<string> _emojis = new List<string> {"k", "u", "r", "w", "a"};
        private readonly IHubContext<MammothHub> _hubContext;
        private readonly ILogger<GreeterService> _logger;
        private readonly Random _random = new Random();

        public GreeterService(ILogger<GreeterService> logger, IHubContext<MammothHub> hubContext,
            IDistributedCache cache)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cache = cache;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var id = 1;
            var currentlyPlayed = await _cache.GetStringAsync($"Currently-Played-{id}");
            await _hubContext.Clients.All.SendAsync("currentlyPlayedChanged",
                new PlayedTrackMessage {ScheduleId = 1, Payload = currentlyPlayed});
            _logger.LogInformation($"currentlyPlayedChanged, {id}");
            // await SendMessage();
            // await SendScheduleChanged(_random.Next(10));
            // _logger.LogInformation($"Invoked onTick, {DateTime.UtcNow}");
            return new HelloReply {Message = request.Name};
        }

        private async Task SendMessage()
        {
            var emoji = _emojis.ElementAt(_random.Next(_emojis.Count));
            await _hubContext.Clients.All.SendAsync("onTick", new {Message = emoji});
        }

        private async Task SendScheduleChanged(int id)
        {
            _logger.LogInformation($"Sending schedule #{id}");
            var schedule = await _cache.GetStringAsync($"{DateTime.Now.Date}-{id}");
            await _hubContext.Clients.All.SendAsync("scheduleChanged",
                new ScheduleMessage {ScheduleId = id, Payload = schedule});
            _logger.LogInformation($"Sent schedule #{id} successfully");
        }
    }
}