using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspireApp.WeatherAPI.Services;

public interface IWeatherService
{
    Task<WeatherResponse?> GetCurrentWeatherAsync(string query, string? lang = "en");
    Task<WeatherResponse?> GetForecastAsync(string query, int days = 7, string? lang = "en");
    Task<SearchResponse?> SearchLocationAsync(string query);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string query, string? lang = "en")
    {
        try
        {
            var apiKey = _configuration["WeatherApi:ApiKey"] ?? Environment.GetEnvironmentVariable("WEATHERAPI_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("WeatherAPI key not configured. Set it in user secrets or environment variable WEATHERAPI_KEY");
                return null;
            }

            var url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={Uri.EscapeDataString(query)}&lang={lang}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<WeatherResponse>(json, options);
            }
            
            _logger.LogWarning("WeatherAPI request failed: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current weather for {Query}", query);
            return null;
        }
    }

    public async Task<WeatherResponse?> GetForecastAsync(string query, int days = 7, string? lang = "en")
    {
        try
        {
            var apiKey = _configuration["WeatherApi:ApiKey"] ?? Environment.GetEnvironmentVariable("WEATHERAPI_KEY");
            _logger.LogInformation("API Key found: {HasKey}", !string.IsNullOrEmpty(apiKey));
            
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("WeatherAPI key not configured. Set it in user secrets or environment variable WEATHERAPI_KEY");
                return null;
            }

            var url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={Uri.EscapeDataString(query)}&days={days}&lang={lang}";
            _logger.LogInformation("Making request to: {Url}", url.Replace(apiKey, "***"));
            
            var response = await _httpClient.GetAsync(url);
            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response received, length: {Length}", json.Length);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<WeatherResponse>(json, options);
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("WeatherAPI forecast request failed: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast for {Query}", query);
            return null;
        }
    }

    public async Task<SearchResponse?> SearchLocationAsync(string query)
    {
        try
        {
            var apiKey = _configuration["WeatherApi:ApiKey"] ?? Environment.GetEnvironmentVariable("WEATHERAPI_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("WeatherAPI key not configured. Set it in user secrets or environment variable WEATHERAPI_KEY");
                return null;
            }

            var url = $"https://api.weatherapi.com/v1/search.json?key={apiKey}&q={Uri.EscapeDataString(query)}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<SearchResponse>(json, options);
            }
            
            _logger.LogWarning("WeatherAPI search request failed: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching location {Query}", query);
            return null;
        }
    }
}

// Weather API Response Models
public class WeatherResponse
{
    public WeatherLocation? Location { get; set; }
    public Current? Current { get; set; }
    public Forecast? Forecast { get; set; }
}

public class WeatherLocation
{
    public string? Name { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string? TzId { get; set; }
    public long LocaltimeEpoch { get; set; }
    public string? Localtime { get; set; }
}

public class Current
{
    [JsonPropertyName("last_updated_epoch")]
    public long LastUpdatedEpoch { get; set; }
    [JsonPropertyName("last_updated")]
    public string? LastUpdated { get; set; }
    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }
    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }
    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
    public Condition? Condition { get; set; }
    [JsonPropertyName("wind_mph")]
    public double WindMph { get; set; }
    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }
    [JsonPropertyName("wind_degree")]
    public int WindDegree { get; set; }
    [JsonPropertyName("wind_dir")]
    public string? WindDir { get; set; }
    [JsonPropertyName("pressure_mb")]
    public double PressureMb { get; set; }
    [JsonPropertyName("pressure_in")]
    public double PressureIn { get; set; }
    [JsonPropertyName("precip_mm")]
    public double PrecipMm { get; set; }
    [JsonPropertyName("precip_in")]
    public double PrecipIn { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    [JsonPropertyName("feelslike_c")]
    public double FeelslikeC { get; set; }
    [JsonPropertyName("feelslike_f")]
    public double FeelslikeF { get; set; }
    [JsonPropertyName("windchill_c")]
    public double WindchillC { get; set; }
    [JsonPropertyName("windchill_f")]
    public double WindchillF { get; set; }
    [JsonPropertyName("heatindex_c")]
    public double HeatindexC { get; set; }
    [JsonPropertyName("heatindex_f")]
    public double HeatindexF { get; set; }
    [JsonPropertyName("dewpoint_c")]
    public double DewpointC { get; set; }
    [JsonPropertyName("dewpoint_f")]
    public double DewpointF { get; set; }
    [JsonPropertyName("vis_km")]
    public double VisKm { get; set; }
    [JsonPropertyName("vis_miles")]
    public double VisMiles { get; set; }
    public double Uv { get; set; }
    [JsonPropertyName("gust_mph")]
    public double GustMph { get; set; }
    [JsonPropertyName("gust_kph")]
    public double GustKph { get; set; }
    [JsonPropertyName("short_rad")]
    public double ShortRad { get; set; }
    [JsonPropertyName("diff_rad")]
    public double DiffRad { get; set; }
    public double Dni { get; set; }
    public double Gti { get; set; }
    [JsonPropertyName("air_quality")]
    public AirQuality? AirQuality { get; set; }
}

public class Condition
{
    public string? Text { get; set; }
    public string? Icon { get; set; }
    public int Code { get; set; }
}

public class AirQuality
{
    public double Co { get; set; }
    public double No2 { get; set; }
    public double O3 { get; set; }
    public double So2 { get; set; }
    public double Pm25 { get; set; }
    public double Pm10 { get; set; }
    public int UsEpaIndex { get; set; }
    public int GbDefraIndex { get; set; }
}

public class Forecast
{
    public List<ForecastDay>? Forecastday { get; set; }
}

public class ForecastDay
{
    public string? Date { get; set; }
    public long DateEpoch { get; set; }
    public Day? Day { get; set; }
    public Astro? Astro { get; set; }
    public List<Hour>? Hour { get; set; }
}

public class Day
{
    [JsonPropertyName("maxtemp_c")]
    public double MaxtempC { get; set; }
    [JsonPropertyName("maxtemp_f")]
    public double MaxtempF { get; set; }
    [JsonPropertyName("mintemp_c")]
    public double MintempC { get; set; }
    [JsonPropertyName("mintemp_f")]
    public double MintempF { get; set; }
    [JsonPropertyName("avgtemp_c")]
    public double AvgtempC { get; set; }
    [JsonPropertyName("avgtemp_f")]
    public double AvgtempF { get; set; }
    [JsonPropertyName("maxwind_mph")]
    public double MaxwindMph { get; set; }
    [JsonPropertyName("maxwind_kph")]
    public double MaxwindKph { get; set; }
    [JsonPropertyName("totalprecip_mm")]
    public double TotalprecipMm { get; set; }
    [JsonPropertyName("totalprecip_in")]
    public double TotalprecipIn { get; set; }
    [JsonPropertyName("totalsnow_cm")]
    public double TotalsnowCm { get; set; }
    [JsonPropertyName("avgvis_km")]
    public double AvgvisKm { get; set; }
    [JsonPropertyName("avgvis_miles")]
    public double AvgvisMiles { get; set; }
    [JsonPropertyName("avghumidity")]
    public double Avghumidity { get; set; }
    [JsonPropertyName("daily_will_it_rain")]
    public int DailyWillItRain { get; set; }
    [JsonPropertyName("daily_chance_of_rain")]
    public int DailyChanceOfRain { get; set; }
    [JsonPropertyName("daily_will_it_snow")]
    public int DailyWillItSnow { get; set; }
    [JsonPropertyName("daily_chance_of_snow")]
    public int DailyChanceOfSnow { get; set; }
    public Condition? Condition { get; set; }
    public double Uv { get; set; }
}

public class Astro
{
    public string? Sunrise { get; set; }
    public string? Sunset { get; set; }
    public string? Moonrise { get; set; }
    public string? Moonset { get; set; }
    public string? MoonPhase { get; set; }
    public int MoonIllumination { get; set; }
    public int IsMoonUp { get; set; }
    public int IsSunUp { get; set; }
}

public class Hour
{
    [JsonPropertyName("time_epoch")]
    public long TimeEpoch { get; set; }
    public string? Time { get; set; }
    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }
    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }
    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
    public Condition? Condition { get; set; }
    [JsonPropertyName("wind_mph")]
    public double WindMph { get; set; }
    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }
    [JsonPropertyName("wind_degree")]
    public int WindDegree { get; set; }
    [JsonPropertyName("wind_dir")]
    public string? WindDir { get; set; }
    [JsonPropertyName("pressure_mb")]
    public double PressureMb { get; set; }
    [JsonPropertyName("pressure_in")]
    public double PressureIn { get; set; }
    [JsonPropertyName("precip_mm")]
    public double PrecipMm { get; set; }
    [JsonPropertyName("precip_in")]
    public double PrecipIn { get; set; }
    [JsonPropertyName("snow_cm")]
    public double SnowCm { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    [JsonPropertyName("feelslike_c")]
    public double FeelslikeC { get; set; }
    [JsonPropertyName("feelslike_f")]
    public double FeelslikeF { get; set; }
    [JsonPropertyName("windchill_c")]
    public double WindchillC { get; set; }
    [JsonPropertyName("windchill_f")]
    public double WindchillF { get; set; }
    [JsonPropertyName("heatindex_c")]
    public double HeatindexC { get; set; }
    [JsonPropertyName("heatindex_f")]
    public double HeatindexF { get; set; }
    [JsonPropertyName("dewpoint_c")]
    public double DewpointC { get; set; }
    [JsonPropertyName("dewpoint_f")]
    public double DewpointF { get; set; }
    [JsonPropertyName("will_it_rain")]
    public int WillItRain { get; set; }
    [JsonPropertyName("chance_of_rain")]
    public int ChanceOfRain { get; set; }
    [JsonPropertyName("will_it_snow")]
    public int WillItSnow { get; set; }
    [JsonPropertyName("chance_of_snow")]
    public int ChanceOfSnow { get; set; }
    [JsonPropertyName("vis_km")]
    public double VisKm { get; set; }
    [JsonPropertyName("vis_miles")]
    public double VisMiles { get; set; }
    [JsonPropertyName("gust_mph")]
    public double GustMph { get; set; }
    [JsonPropertyName("gust_kph")]
    public double GustKph { get; set; }
    public double Uv { get; set; }
    [JsonPropertyName("short_rad")]
    public double ShortRad { get; set; }
    [JsonPropertyName("diff_rad")]
    public double DiffRad { get; set; }
    public double Dni { get; set; }
    public double Gti { get; set; }
}

public class SearchResponse : List<SearchLocation>
{
}

public class SearchLocation
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string? Url { get; set; }
} 