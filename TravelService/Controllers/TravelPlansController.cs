using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelService.DTOs;
using TravelService.Services;

namespace TravelService.Controllers
{
    [ApiController]
    [Route("api/travel-plans")]
    [Authorize]
    public class TravelPlansController : ControllerBase
    {
        private readonly TravelPlanService _service;

        public TravelPlansController(TravelPlanService service)
        {
            _service = service;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync(GetUserId()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, GetUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");
            if (dto.Budget < 0)
                return BadRequest("Budget cannot be negative.");

            var result = await _service.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTravelPlanDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest("End date cannot be before start date.");
            if (dto.Budget < 0)
                return BadRequest("Budget cannot be negative.");

            var result = await _service.UpdateAsync(id, dto, GetUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id, GetUserId());
            return result ? NoContent() : NotFound();
        }

        // Destinations
        [HttpPost("{planId}/destinations")]
        public async Task<IActionResult> AddDestination(int planId, [FromBody] CreateDestinationDto dto)
        {
            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Departure date cannot be before arrival date.");

            var result = await _service.AddDestinationAsync(planId, dto, GetUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{planId}/destinations/{destinationId}")]
        public async Task<IActionResult> DeleteDestination(int planId, int destinationId)
        {
            var result = await _service.DeleteDestinationAsync(destinationId, GetUserId());
            return result ? NoContent() : NotFound();
        }

        // Activities
        [HttpPost("{planId}/activities")]
        public async Task<IActionResult> AddActivity(int planId, [FromBody] CreateActivityDto dto)
        {
            var result = await _service.AddActivityAsync(planId, dto, GetUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{planId}/activities/{activityId}")]
        public async Task<IActionResult> DeleteActivity(int planId, int activityId)
        {
            var result = await _service.DeleteActivityAsync(activityId, GetUserId());
            return result ? NoContent() : NotFound();
        }

        // Public endpoints
        [HttpGet("public/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublic(int id)
        {
            var result = await _service.GetByIdPublicAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("public/{planId}/destinations")]
        [AllowAnonymous]
        public async Task<IActionResult> AddDestinationPublic(int planId, [FromBody] CreateDestinationDto dto)
        {
            if (dto.DepartureDate < dto.ArrivalDate)
                return BadRequest("Departure date cannot be before arrival date.");
            var result = await _service.AddDestinationPublicAsync(planId, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("public/{planId}/activities")]
        [AllowAnonymous]
        public async Task<IActionResult> AddActivityPublic(int planId, [FromBody] CreateActivityDto dto)
        {
            var result = await _service.AddActivityPublicAsync(planId, dto);
            return result == null ? NotFound() : Ok(result);
        }
    }
}