using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Mammoth.Core.Entities
{
    public class Playlist
    {
        public Playlist()
        {
            _timer = new Timer {Interval = 100};
            _timer.Elapsed += OnTimerOnElapsed;
            _timer.Start();
            _channels = new Dictionary<int, Stack<Track>>();
        }

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args)
        {
            foreach (var (key, value) in _channels)
            {
                if (!value.TryPeek(out var current)) continue;
                if (current.End > DateTime.Now) continue;
                value.TryPop(out _);
                OnTrackChanged(key, value.Peek());
                Console.WriteLine("Track has expired, popping");
            }
        }

        private readonly IDictionary<int, Stack<Track>> _channels;
        private readonly Timer _timer;
        public event EventHandler<TrackChange> TrackChanged;


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

            var hasTrack = _channels[channelId].TryPeek(out var track);
            if (hasTrack)
            {
                return track;
            }

            throw new ApplicationException("Channel is empty");
        }

        protected virtual void OnTrackChanged(int channelId, Track track)
        {
            TrackChanged?.Invoke(this, new TrackChange
            {
                Track = track,
                ChannelId = channelId
            });
        }
    }
}