using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Mammoth.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> Get() => Ok("schedules");


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSchedule(int id, DateTime date)
        {
            var key = $"{DateTime.Now.Date}-{id}";
            var schedule = await _cache.GetStringAsync(key);
            return Ok(schedule);
        }
    }
}