using System;
using System.Collections;
using System.Collections.Generic;

namespace Mammoth.Core.Entities
{
    public class Track
    {
        public int AntenaId { get; }
        public string ArticleLink { get; }
        public string Category { get; }
        public string Description { get; }
        public int Id { get; }
        public string Title { get; }
        public bool IsActive { get; }
        public IEnumerable<Leader> Leaders { get; }
        public string Photo { get; }
        public IEnumerable<Sound> Sounds { get; }
        public DateTime StartHour { get; }
        public DateTime StopHour { get; }

        public Track(int antenaId, string articleLink, string category, string description, int id, string title,
            bool isActive, dynamic leaders, string photo, dynamic sounds, DateTime startHour, DateTime stopHour)
        {
            AntenaId = antenaId;
            ArticleLink = articleLink;
            Category = category;
            Description = description;
            Id = id;
            Title = title;
            IsActive = isActive;
            Leaders = leaders;
            Photo = photo;
            Sounds = sounds;
            StartHour = startHour;
            StopHour = stopHour;
        }
    }
}