using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BS.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ValuesController> logger;
        private readonly DataContext context;

        public ValuesController(
            DataContext context,
            ILogger<ValuesController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await context.Values.ToListAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // [HttpGet]
        // public IEnumerable<WeatherForecast> Get()
        // {
        //     //throw new Exception("TEst exception");
        //     var rng = new Random();
        //     return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //     {
        //         Date = DateTime.Now.AddDays(index),
        //         TemperatureC = rng.Next(-20, 55),
        //         Summary = Summaries[rng.Next(Summaries.Length)]
        //     })
        //     .ToArray();
        // }
    }
}
