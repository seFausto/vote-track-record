using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Propublica;

namespace VotingTrackRecord.Controllers
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
        private readonly IPropublicaService propublicaService;
        public WeatherForecastController(IPropublicaService propublicaService,  ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            this.propublicaService = propublicaService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            propublicaService.GetRecentVotesAsync("house");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}