using System.Collections.Generic;

namespace Mammoth.Worker.DTO
{
    public class ScheduleResponse
    {
        public IEnumerable<Program> Schedule { get; set; }
    }
}