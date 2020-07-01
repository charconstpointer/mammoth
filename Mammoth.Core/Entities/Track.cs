using System;

namespace Mammoth.Core.Entities
{
    public class Track
    {
        public Track(string title)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }

        public string Title { get;  }
    }
}