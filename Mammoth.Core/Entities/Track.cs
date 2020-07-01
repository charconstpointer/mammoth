using System;

namespace Mammoth.Core.Entities
{
    public class Track
    {
        public Track(string title, DateTime start, DateTime end)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Start = start;
            End = end;
        }

        public string Title { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
    }
}