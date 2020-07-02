using System;
using Mammoth.Core.Entities;

namespace Mammoth.Core.Events
{
    public class TrackChange : EventArgs
    {
        public int ChannelId { get; set; }
        public Track Track { get; set; }
    }
}