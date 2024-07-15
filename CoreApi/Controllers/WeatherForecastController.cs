using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CoreApi.Controllers
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

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get333()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet(Name = "GetWeatherForecast123")]
        public IActionResult Get()
        {
            return BadRequest("abec123");
        }
        //[HttpGet(Name = "GetWeatherForecast")]
        //public IActionResult Get2()
        //{
        //    return BadRequest("abec1232");
        //}

        //[HttpGet(Name = "GetWeatherForecast2")]
        //public IActionResult Get2()
        //{
        //    return StatusCode((int)HttpStatusCode.BadRequest, "abec");
        //}

        //[HttpGet(Name = "GetWeatherForecast3")]
        //public IActionResult Get3()
        //{
        //    return StatusCode((int)HttpStatusCode.OK, "abec");
        //}

        //[HttpGet(Name = "GetWeatherForecast4")]
        //public IActionResult Get4()
        //{
        //    return Ok("abec");
        //}
    }
}
