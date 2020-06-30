using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Mammoth.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Mammoth.Api.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IHubContext<MammothHub> _hubContext;
        private readonly Random _random = new Random();
        private readonly ICollection<string> _emojis = new List<string> {"k", "u", "r", "w", "a"};

        public GreeterService(ILogger<GreeterService> logger, IHubContext<MammothHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Tick, {DateTime.UtcNow}");
            await SendMessage();
            _logger.LogInformation($"Invoked onTick, {DateTime.UtcNow}");
            return new HelloReply {Message = request.Name};
        }

        private async Task SendMessage()
        {
            var emoji = _emojis.ElementAt(_random.Next(_emojis.Count ));
            await _hubContext.Clients.All.SendAsync("onTick", new {Message = emoji});
        }
    }
}