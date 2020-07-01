using System;
using System.Collections.Generic;
using System.Linq;

namespace Mammoth.Core.Entities
{
    public class Playlist
    {
        public Playlist()
        {
            _channels = new Dictionary<int, Stack<Track>>();
        }

        private readonly IDictionary<int, Stack<Track>> _channels;

        public void AddChannel(int channelId, IEnumerable<Track> tracks)
        {
            if (_channels.ContainsKey(channelId))
            {
                throw new ApplicationException("Channel already exists");
            }

            _channels.Add(channelId, new Stack<Track>());
            var channel = _channels[channelId];
            foreach (var track in tracks.Reverse())
            {
                channel.Push(track);
            }
        }

        public Track GetCurrentlyPlayed(int channelId)
        {
            if (!_channels.ContainsKey(channelId))
            {
                throw new ApplicationException("No such channel");
            }

            var hasTrack = _channels[channelId].TryPop(out var track);
            if (hasTrack)
            {
                return track;
            }
            throw new ApplicationException("Channel is empty");
        }
    }
}