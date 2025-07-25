using Microsoft.AspNetCore.Mvc;
using AspireApp.MasterDataService.Models;
using AspireApp.MasterDataService.Interfaces;
using Wolverine;
using AspireApp.MasterDataService.Messages.Commands;

namespace AspireApp.MasterDataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IMessageBus _bus;

        public LocationsController(ILocationService locationService, IMessageBus bus)
        {
            _locationService = locationService;
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _locationService.GetAllAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null)
                return NotFound();

            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
        {
            var id = await _bus.InvokeAsync<int>(command);
            return CreatedAtAction(nameof(GetById), new { id }, command);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Location location)
        {
            if (id != location.Id)
                return BadRequest();

            var updated = await _locationService.UpdateAsync(location);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _locationService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}