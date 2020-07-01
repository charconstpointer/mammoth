using System;
using System.Threading.Tasks;
using Mammoth.Core.Entities;

namespace Mammoth.Runner
{
    internal class App
    {
        private static async Task Main(string[] args)
        {
            var playlist = new Playlist();
            playlist.TrackChanged += OnPlaylistOnTrackChanged;
            Console.ReadKey();
        }

        private static void OnPlaylistOnTrackChanged(object? sender, TrackChange eventArgs)
        {
            Console.WriteLine($"Track {eventArgs.Track.Title} is playing on {eventArgs.ChannelId}");
        }
    }
}