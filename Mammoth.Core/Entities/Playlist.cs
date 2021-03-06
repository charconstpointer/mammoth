using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Mammoth.Core.Events;

namespace Mammoth.Core.Entities
{
    public class Playlist
    {
        private readonly IDictionary<int, Stack<Track>> _channels;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _ticker;

        public Playlist()
        {
            _channels = new ConcurrentDictionary<int, Stack<Track>>();
            _ticker = new Thread(OnTick) {IsBackground = true};
            _ticker.Start();
        }

        private void OnTick()
        {
            while (true)
            {
                foreach (var (key, value) in _channels)
                {
                    if (!value.TryPeek(out var current)) continue;
                    if (current.StopHour > DateTime.UtcNow.AddHours(2)) continue;
                    value.TryPop(out _);
                    OnTrackChanged(key, value.Peek());
                    Console.WriteLine("Track has expired, popping");
                }

                Thread.Sleep(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public event EventHandler<TrackChange> TrackChanged;


        public void AddChannel(int channelId, IEnumerable<Track> tracks)
        {
            if (_channels.ContainsKey(channelId)) throw new ApplicationException("Channel already exists");

            _channels.Add(channelId, new Stack<Track>());
            var channel = _channels[channelId];
            var channelSchedule = tracks.ToList();
            var tracksToAdd = channelSchedule.Where(t => t.StopHour >= DateTime.UtcNow.AddHours(2));
            foreach (var track in tracksToAdd.Reverse()) channel.Push(track);
        }

        public Track GetCurrentlyPlayed(int channelId)
        {
            if (!_channels.ContainsKey(channelId)) throw new ApplicationException("No such channel");

            var hasTrack = _channels[channelId].TryPeek(out var track);
            if (hasTrack) return track;

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