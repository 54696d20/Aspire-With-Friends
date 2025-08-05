using Microsoft.AspNetCore.Mvc;
using AspireApp.MasterDataService.Models;
using Wolverine;
using AspireApp.MasterDataService.Messages.Commands;
using AspireApp.MasterDataService.Messages.Queries;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AspireApp.MasterDataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(IMessageBus bus, ILogger<LocationsController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/locations - Retrieving all locations");
            var locations = await _bus.InvokeAsync<IEnumerable<Location>>(new GetAllLocationsQuery());
            _logger.LogInformation("GET /api/locations - Retrieved {LocationCount} locations", locations.Count());
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/locations/{LocationId} - Retrieving location", id);
            var location = await _bus.InvokeAsync<Location?>(new GetLocationByIdQuery(id));
            if (location == null)
            {
                _logger.LogWarning("GET /api/locations/{LocationId} - Location not found", id);
                return NotFound();
            }

            _logger.LogInformation("GET /api/locations/{LocationId} - Location found: {LocationName}", id, location.Name);
            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
        {
            _logger.LogInformation("POST /api/locations - Creating location: {LocationName}", command.Name);
            try
            {
                var id = await _bus.InvokeAsync<int>(command);
                _logger.LogInformation("POST /api/locations - Location created with ID: {LocationId}", id);
                return CreatedAtAction(nameof(GetById), new { id }, command);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("POST /api/locations - Validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(new { errors });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            _logger.LogInformation("PUT /api/locations/{LocationId} - Updating location: {LocationName}", id, command.Name);
            try
            {
                var updated = await _bus.InvokeAsync<bool>(command);
                if (!updated)
                {
                    _logger.LogWarning("PUT /api/locations/{LocationId} - Location not found", id);
                    return NotFound();
                }

                _logger.LogInformation("PUT /api/locations/{LocationId} - Location updated successfully", id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("PUT /api/locations/{LocationId} - Validation failed: {Errors}", id, string.Join(", ", errors));
                return BadRequest(new { errors });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/locations/{LocationId} - Deleting location", id);
            var deleted = await _bus.InvokeAsync<bool>(new DeleteLocationCommand(id));
            if (!deleted)
            {
                _logger.LogWarning("DELETE /api/locations/{LocationId} - Location not found", id);
                return NotFound();
            }

            _logger.LogInformation("DELETE /api/locations/{LocationId} - Location deleted successfully", id);
            return NoContent();
        }
    }
}