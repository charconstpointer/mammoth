using System;
using System.Collections.Generic;

namespace Mammoth.Worker.DTO
{
    public class SubProgram
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime StopHour { get; set; }
        public IEnumerable<Leader> Leaders { get; set; }
        public string Photo { get; set; }
        public IEnumerable<Sound> Sounds { get; set; }
    }
}