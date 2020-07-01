using System;
using System.Collections.Generic;

namespace Mammoth.Api.DTO
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

        protected bool Equals(Program other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Program) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}