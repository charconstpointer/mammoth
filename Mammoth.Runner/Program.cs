using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mammoth.Core.Entities;

namespace Mammoth.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var playlist = new Playlist();
            // playlist.AddChannel(1, new List<Track>
            // {
            //     new Track("first", DateTime.Now, DateTime.Now.AddSeconds(10) ),
            //     new Track("second", DateTime.Now.AddSeconds(10), DateTime.Now.AddSeconds(20)),
            //     new Track("third", DateTime.Now.AddSeconds(20), DateTime.Now.AddSeconds(30)),
            // });
            playlist.TrackChanged += OnPlaylistOnTrackChanged;
            // while (playlist.GetCurrentlyPlayed(1).Title != null)
            // {
            //     Console.WriteLine(playlist.GetCurrentlyPlayed(1).Title);
            //     await Task.Delay(1000);
            // }
            Console.ReadKey();
        }

        private static void OnPlaylistOnTrackChanged(object? sender, TrackChange eventArgs)
        {
            Console.WriteLine($"Track { eventArgs.Track.Title} is playing on {eventArgs.ChannelId}");
        }
    }
}