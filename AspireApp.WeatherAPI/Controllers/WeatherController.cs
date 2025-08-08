using Microsoft.AspNetCore.Mvc;
using AspireApp.WeatherAPI.Services;
using System.Text.Json;

namespace AspireApp.WeatherAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("current/{query}")]
    public async Task<IActionResult> GetCurrentWeather(string query, [FromQuery] string? lang = "en")
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(query, lang);
            return weather != null ? Ok(weather) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("forecast/{query}")]
    public async Task<IActionResult> GetForecast(string query, [FromQuery] int days = 7, [FromQuery] string? lang = "en")
    {
        _logger.LogError("Getting the info for you!!!!!!!!!!!!!!!");
        try
        {
            var weather = await _weatherService.GetForecastAsync(query, days, lang);
            return weather != null ? Ok(weather) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast for {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchLocation(string query)
    {
        try
        {
            var locations = await _weatherService.SearchLocationAsync(query);
            return locations != null ? Ok(locations) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching location {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("debug")]
    public IActionResult Debug([FromServices] IConfiguration config)
    {
        var apiKey = config["WeatherApi:ApiKey"];
        return Ok(new { 
            HasApiKey = !string.IsNullOrEmpty(apiKey), 
            ApiKeyLength = apiKey?.Length ?? 0 
        });
    }

                    [HttpGet("test-external")]
                public async Task<IActionResult> TestExternal([FromServices] IConfiguration config, [FromServices] HttpClient httpClient)
                {
                    try
                    {
                        var apiKey = config["WeatherApi:ApiKey"];
                        if (string.IsNullOrEmpty(apiKey))
                        {
                            return BadRequest("API key not found");
                        }

                        var url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q=23323&days=7";
                        var response = await httpClient.GetAsync(url);
                        var content = await response.Content.ReadAsStringAsync();

                        // Try to deserialize and show the result
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content, options);

                        return Ok(new {
                            StatusCode = (int)response.StatusCode,
                            ContentLength = content.Length,
                            RawContent = content.Substring(0, Math.Min(500, content.Length)),
                            DeserializedCurrent = weatherResponse?.Current != null ? new {
                                TempC = weatherResponse.Current.TempC,
                                TempF = weatherResponse.Current.TempF,
                                FeelslikeC = weatherResponse.Current.FeelslikeC,
                                FeelslikeF = weatherResponse.Current.FeelslikeF
                            } : null,
                            DeserializedDay = weatherResponse?.Forecast?.Forecastday?.FirstOrDefault()?.Day != null ? new {
                                MaxtempC = weatherResponse.Forecast.Forecastday[0].Day.MaxtempC,
                                MaxtempF = weatherResponse.Forecast.Forecastday[0].Day.MaxtempF,
                                MintempC = weatherResponse.Forecast.Forecastday[0].Day.MintempC,
                                MintempF = weatherResponse.Forecast.Forecastday[0].Day.MintempF
                            } : null
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error testing external API");
                        return StatusCode(500, "Internal server error");
                    }
                }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("WeatherAPI is healthy");
    }
} 