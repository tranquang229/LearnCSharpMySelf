using CoreWebApi.Authorized;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[EOAuthorizeAttribute]
        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            return BadRequest("abec");
        }

        [HttpGet(Name = "GetWeatherForecast2")]
        public IActionResult Get2()
        {
            return StatusCode((int)HttpStatusCode.BadRequest, "abec");
        }

        [HttpGet(Name = "GetWeatherForecast3")]
        public IActionResult Get3()
        {
            return StatusCode((int)HttpStatusCode.OK, "abec");
        }

        [HttpGet(Name = "GetWeatherForecast4")]
        public IActionResult Get4()
        {
            return Ok("abec");
        }
    }
}
