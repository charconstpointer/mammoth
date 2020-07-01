using System;
using System.Collections.Generic;
using Mammoth.Core.Entities;

namespace Mammoth.Worker.DTO
{
    public class Program
    {
        public int AntenaId { get; set; }
        public string ArticleLink { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Leader> Leaders { get; set; }
        public string Photo { get; set; }
        public IEnumerable<Sound> Sounds { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime StopHour { get; set; }
        public IEnumerable<SubProgram> Subprograms { get; set; }

    }
}