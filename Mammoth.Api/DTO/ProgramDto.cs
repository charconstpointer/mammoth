using System;

namespace Mammoth.Api.DTO
{
    public class ProgramDto
    {
        public int AntenaId { get; set; }
        public string ArticleLink { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public dynamic Leaders { get; set; }
        public string Photo { get; set; }
        public dynamic Sounds { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime StopHour { get; set; }
    }
}