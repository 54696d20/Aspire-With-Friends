using System.Net.Http.Json;
using System.Text.Json;

namespace AspireApp.WebWasm.Services;

public interface ILocationService
{
    Task<List<Location>> GetLocationsAsync();
    Task<Location?> GetLocationByIdAsync(int id);
    Task<int> CreateLocationAsync(CreateLocationRequest request);
    Task<bool> UpdateLocationAsync(int id, UpdateLocationRequest request);
    Task<bool> DeleteLocationAsync(int id);
}

public class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public LocationService(HttpClient httpClient, ILogger<LocationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Location>> GetLocationsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching locations from API");
            var response = await _httpClient.GetAsync("http://localhost:5211/masterdata-api/api/locations");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<Location>>(json, _jsonOptions) ?? new List<Location>();
                _logger.LogInformation("Successfully fetched {Count} locations", locations.Count);
                return locations;
            }
            
            _logger.LogWarning("Failed to fetch locations: {StatusCode}", response.StatusCode);
            return new List<Location>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching locations");
            throw;
        }
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching location {LocationId} from API", id);
            var response = await _httpClient.GetAsync($"http://localhost:5211/masterdata-api/api/locations/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var location = JsonSerializer.Deserialize<Location>(json, _jsonOptions);
                _logger.LogInformation("Successfully fetched location {LocationId}", id);
                return location;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Location {LocationId} not found", id);
                return null;
            }
            
            _logger.LogWarning("Failed to fetch location {LocationId}: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching location {LocationId}", id);
            throw;
        }
    }

    public async Task<int> CreateLocationAsync(CreateLocationRequest request)
    {
        try
        {
            _logger.LogInformation("Creating location: {LocationName} ({LocationType})", request.Name, request.Type);
            
            // Convert to the command object that the API expects
            var command = new CreateLocationCommand(request.Name, request.Type, request.ParentId);
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5211/masterdata-api/api/locations", command);
            
            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.Location?.ToString();
                var id = ExtractIdFromLocationHeader(locationHeader);
                _logger.LogInformation("Successfully created location with ID: {LocationId}", id);
                return id;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to create location: {StatusCode} - {Error}", response.StatusCode, errorContent);
            throw new HttpRequestException($"Failed to create location: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating location: {LocationName}", request.Name);
            throw;
        }
    }

    public async Task<bool> UpdateLocationAsync(int id, UpdateLocationRequest request)
    {
        try
        {
            _logger.LogInformation("Updating location {LocationId}: {LocationName} ({LocationType})", id, request.Name, request.Type);
            
            // Convert to the command object that the API expects
            var command = new UpdateLocationCommand(id, request.Name, request.Type, request.ParentId);
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5211/masterdata-api/api/locations/{id}", command);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully updated location {LocationId}", id);
                return true;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Location {LocationId} not found for update", id);
                return false;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update location {LocationId}: {StatusCode} - {Error}", id, response.StatusCode, errorContent);
            throw new HttpRequestException($"Failed to update location: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location {LocationId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting location {LocationId}", id);
            var response = await _httpClient.DeleteAsync($"http://localhost:5211/masterdata-api/api/locations/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted location {LocationId}", id);
                return true;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Location {LocationId} not found for deletion", id);
                return false;
            }
            
            _logger.LogWarning("Failed to delete location {LocationId}: {StatusCode}", id, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting location {LocationId}", id);
            throw;
        }
    }

    private static int ExtractIdFromLocationHeader(string? locationHeader)
    {
        if (string.IsNullOrEmpty(locationHeader))
            return 0;
        
        // Extract ID from location header like "/api/locations/5"
        var segments = locationHeader.Split('/');
        if (segments.Length > 0 && int.TryParse(segments[^1], out var id))
            return id;
        
        return 0;
    }
}

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int? ParentId { get; set; }
}

public class CreateLocationRequest
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int? ParentId { get; set; }
}

public class UpdateLocationRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int? ParentId { get; set; }
}

// Command objects that match the API expectations
public record CreateLocationCommand(string Name, string Type, int? ParentId);
public record UpdateLocationCommand(int Id, string Name, string Type, int? ParentId); 