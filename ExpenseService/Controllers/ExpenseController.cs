using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseService.DTOs;
using ExpenseService.Services;

namespace ExpenseService.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly ExpensePlanService _service;

        public ExpensesController(ExpensePlanService service)
        {
            _service = service;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet("{travelPlanId}")]
        public async Task<IActionResult> GetAll(int travelPlanId) =>
            Ok(await _service.GetAllAsync(travelPlanId, GetUserId()));

        [HttpGet("{travelPlanId}/summary")]
        public async Task<IActionResult> GetSummary(int travelPlanId) =>
            Ok(await _service.GetBudgetSummaryAsync(travelPlanId, GetUserId()));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Amount must be positive.");

            var result = await _service.CreateAsync(dto, GetUserId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExpenseDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Amount must be positive.");

            var result = await _service.UpdateAsync(id, dto, GetUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id, GetUserId());
            return result ? NoContent() : NotFound();
        }

        [HttpPost("share")]
        public async Task<IActionResult> CreateShareToken([FromBody] CreateShareTokenDto dto)
        {
            var result = await _service.CreateShareTokenAsync(dto);
            return Ok(result);
        }

        [HttpGet("share/validate/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var result = await _service.ValidateTokenAsync(token);
            if (result == null) return Unauthorized("Invalid or expired token.");
            return Ok(new { accessType = result.AccessType, travelPlanId = result.TravelPlanId });
        }
    }
}