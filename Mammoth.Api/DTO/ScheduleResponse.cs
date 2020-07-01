using System.Collections.Generic;
using Mammoth.Worker.DTO;

namespace Mammoth.Api.DTO
{
    public class ScheduleResponse
    {
        public IEnumerable<Program> Schedule { get; set; }
    }
}