using System;
using System.Collections.Generic;
using Mammoth.Core.Entities;

namespace Mammoth.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var playlist = new Playlist();
            playlist.AddChannel(1, new List<Track>
            {
                new Track("first"),
                new Track("second"),
                new Track("third"),
            });
            Console.WriteLine(playlist.GetCurrentlyPlayed(1).Title);
            Console.WriteLine(playlist.GetCurrentlyPlayed(1).Title);
            Console.WriteLine(playlist.GetCurrentlyPlayed(1).Title);
        }
    }
}