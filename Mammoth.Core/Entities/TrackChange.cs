using System;

namespace Mammoth.Core.Entities
{
    public class TrackChange : EventArgs
    {
        public int ChannelId { get; set; }
        public Track Track { get; set; }
    }
}