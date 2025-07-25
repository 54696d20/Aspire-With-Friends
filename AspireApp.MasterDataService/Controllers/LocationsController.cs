using Microsoft.AspNetCore.Mvc;
using AspireApp.MasterDataService.Models;
using Wolverine;
using AspireApp.MasterDataService.Messages.Commands;
using AspireApp.MasterDataService.Messages.Queries;

namespace AspireApp.MasterDataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly IMessageBus _bus;

        public LocationsController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _bus.InvokeAsync<IEnumerable<Location>>(new GetAllLocationsQuery());
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _bus.InvokeAsync<Location?>(new GetLocationByIdQuery(id));
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var updated = await _bus.InvokeAsync<bool>(command);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bus.InvokeAsync<bool>(new DeleteLocationCommand(id));
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}