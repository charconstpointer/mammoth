using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Mammoth.Api
{
    [ApiController]
    [Route("[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public SchedulesController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSchedule(int id, DateTime date)
        {
            var schedule = await _cache.GetStringAsync("Mammoth.Test");
            return Ok(schedule);
        }
    }
}