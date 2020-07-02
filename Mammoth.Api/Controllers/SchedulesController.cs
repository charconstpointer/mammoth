using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mammoth.Api.DTO;
using Mammoth.Worker.DTO;
using Mammoth.Worker.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mammoth.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(ILogger<SchedulesController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("schedules");
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSchedule(int id, DateTime date)
        {
            var day = DateTime.Now;
            var response = await _httpClient.GetStringAsync(
                $"https://polskie.azurewebsites.net/mobile/api/schedules/?Program={id}&SelectedDate={day}");
            _logger.LogInformation("Fetched schedule");
            var schedule = JsonConvert.DeserializeObject<ScheduleResponse>(response)
                .Schedule.OrderBy(s => s.StopHour)
                .AsDto();
            return Ok(schedule);
        }
    }
}