using System;
using System.Threading.Tasks;
using Mammoth.Core.Entities;
using Mammoth.Core.Events;

namespace Mammoth.Runner
{
    internal class App
    {
        private static void Main()
        {
            var playlist = new Playlist();
            playlist.TrackChanged += OnPlaylistOnTrackChanged;
            Console.ReadKey();
        }

        private static void OnPlaylistOnTrackChanged(object sender, TrackChange eventArgs)
        {
            Console.WriteLine($"Track {eventArgs.Track.Title} is playing on {eventArgs.ChannelId}");
        }
    }
}