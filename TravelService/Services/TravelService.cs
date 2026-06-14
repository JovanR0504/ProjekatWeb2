using Microsoft.EntityFrameworkCore;
using TravelService.Data;
using TravelService.DTOs;
using TravelService.Models;

namespace TravelService.Services
{
    public class TravelPlanService
    {
        private readonly TravelDbContext _context;

        public TravelPlanService(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<List<TravelPlanResponseDto>> GetAllAsync(int userId)
        {
            return await _context.TravelPlans
                .Where(t => t.UserId == userId)
                .Include(t => t.Destinations)
                .Include(t => t.Activities)
                .Include(t => t.ChecklistItems)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        public async Task<TravelPlanResponseDto?> GetByIdAsync(int id, int userId)
        {
            var plan = await _context.TravelPlans
                .Include(t => t.Destinations)
                .Include(t => t.Activities)
                .Include(t => t.ChecklistItems)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return plan == null ? null : MapToDto(plan);
        }

        public async Task<TravelPlanResponseDto> CreateAsync(CreateTravelPlanDto dto, int userId)
        {
            var plan = new TravelPlan
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Budget = dto.Budget,
                Notes = dto.Notes
            };

            _context.TravelPlans.Add(plan);
            await _context.SaveChangesAsync();
            return MapToDto(plan);
        }

        public async Task<TravelPlanResponseDto?> UpdateAsync(int id, CreateTravelPlanDto dto, int userId)
        {
            var plan = await _context.TravelPlans
                .Include(t => t.Destinations)
                .Include(t => t.Activities)
                .Include(t => t.ChecklistItems)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (plan == null) return null;

            plan.Name = dto.Name;
            plan.Description = dto.Description;
            plan.StartDate = dto.StartDate;
            plan.EndDate = dto.EndDate;
            plan.Budget = dto.Budget;
            plan.Notes = dto.Notes;

            await _context.SaveChangesAsync();
            return MapToDto(plan);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (plan == null) return false;

            _context.TravelPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        // Destinations
        public async Task<DestinationResponseDto?> AddDestinationAsync(int planId, CreateDestinationDto dto, int userId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(t => t.Id == planId && t.UserId == userId);

            if (plan == null) return null;

            var destination = new Destination
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Location = dto.Location,
                ArrivalDate = dto.ArrivalDate,
                DepartureDate = dto.DepartureDate,
                Description = dto.Description,
                Notes = dto.Notes
            };

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return new DestinationResponseDto
            {
                Id = destination.Id,
                TravelPlanId = destination.TravelPlanId,
                Name = destination.Name,
                Location = destination.Location,
                ArrivalDate = destination.ArrivalDate,
                DepartureDate = destination.DepartureDate,
                Description = destination.Description,
                Notes = destination.Notes
            };
        }

        public async Task<bool> DeleteDestinationAsync(int destinationId, int userId)
        {
            var destination = await _context.Destinations
                .Include(d => d.TravelPlan)
                .FirstOrDefaultAsync(d => d.Id == destinationId && d.TravelPlan.UserId == userId);

            if (destination == null) return false;

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            return true;
        }

        // Activities
        public async Task<ActivityResponseDto?> AddActivityAsync(int planId, CreateActivityDto dto, int userId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(t => t.Id == planId && t.UserId == userId);

            if (plan == null) return null;

            var activity = new Activity
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Date = dto.Date,
                Time = dto.Time,
                Location = dto.Location,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                Status = dto.Status
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return new ActivityResponseDto
            {
                Id = activity.Id,
                TravelPlanId = activity.TravelPlanId,
                Name = activity.Name,
                Date = activity.Date,
                Time = activity.Time,
                Location = activity.Location,
                Description = activity.Description,
                EstimatedCost = activity.EstimatedCost,
                Status = activity.Status
            };
        }

        public async Task<bool> DeleteActivityAsync(int activityId, int userId)
        {
            var activity = await _context.Activities
                .Include(a => a.TravelPlan)
                .FirstOrDefaultAsync(a => a.Id == activityId && a.TravelPlan.UserId == userId);

            if (activity == null) return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Public methods
        public async Task<TravelPlanResponseDto?> GetByIdPublicAsync(int id)
        {
            var plan = await _context.TravelPlans
                .Include(t => t.Destinations)
                .Include(t => t.Activities)
                .Include(t => t.ChecklistItems)
                .FirstOrDefaultAsync(t => t.Id == id);

            return plan == null ? null : MapToDto(plan);
        }

        public async Task<DestinationResponseDto?> AddDestinationPublicAsync(int planId, CreateDestinationDto dto)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(t => t.Id == planId);
            if (plan == null) return null;

            var destination = new Destination
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Location = dto.Location,
                ArrivalDate = dto.ArrivalDate,
                DepartureDate = dto.DepartureDate,
                Description = dto.Description,
                Notes = dto.Notes
            };

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return new DestinationResponseDto
            {
                Id = destination.Id,
                TravelPlanId = destination.TravelPlanId,
                Name = destination.Name,
                Location = destination.Location,
                ArrivalDate = destination.ArrivalDate,
                DepartureDate = destination.DepartureDate,
                Description = destination.Description,
                Notes = destination.Notes
            };
        }

        public async Task<ActivityResponseDto?> AddActivityPublicAsync(int planId, CreateActivityDto dto)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(t => t.Id == planId);
            if (plan == null) return null;

            var activity = new Activity
            {
                TravelPlanId = planId,
                Name = dto.Name,
                Date = dto.Date,
                Time = dto.Time,
                Location = dto.Location,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                Status = dto.Status
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return new ActivityResponseDto
            {
                Id = activity.Id,
                TravelPlanId = activity.TravelPlanId,
                Name = activity.Name,
                Date = activity.Date,
                Time = activity.Time,
                Location = activity.Location,
                Description = activity.Description,
                EstimatedCost = activity.EstimatedCost,
                Status = activity.Status
            };
        }

        private static TravelPlanResponseDto MapToDto(TravelPlan t) => new()
        {
            Id = t.Id,
            UserId = t.UserId,
            Name = t.Name,
            Description = t.Description,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Budget = t.Budget,
            Notes = t.Notes,
            CreatedAt = t.CreatedAt,
            Destinations = t.Destinations.Select(d => new DestinationResponseDto
            {
                Id = d.Id,
                TravelPlanId = d.TravelPlanId,
                Name = d.Name,
                Location = d.Location,
                ArrivalDate = d.ArrivalDate,
                DepartureDate = d.DepartureDate,
                Description = d.Description,
                Notes = d.Notes
            }).ToList(),
            Activities = t.Activities.Select(a => new ActivityResponseDto
            {
                Id = a.Id,
                TravelPlanId = a.TravelPlanId,
                Name = a.Name,
                Date = a.Date,
                Time = a.Time,
                Location = a.Location,
                Description = a.Description,
                EstimatedCost = a.EstimatedCost,
                Status = a.Status
            }).ToList(),
            ChecklistItems = t.ChecklistItems.Select(c => new ChecklistItemResponseDto
            {
                Id = c.Id,
                TravelPlanId = c.TravelPlanId,
                Name = c.Name,
                IsCompleted = c.IsCompleted
            }).ToList()
        };
        // Checklist
        public async Task<ChecklistItemResponseDto?> AddChecklistItemAsync(int planId, CreateChecklistItemDto dto, int userId)
        {
            var plan = await _context.TravelPlans
                .FirstOrDefaultAsync(t => t.Id == planId && t.UserId == userId);

            if (plan == null) return null;

            var item = new ChecklistItem
            {
                TravelPlanId = planId,
                Name = dto.Name,
                IsCompleted = false
            };

            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();

            return new ChecklistItemResponseDto
            {
                Id = item.Id,
                TravelPlanId = item.TravelPlanId,
                Name = item.Name,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<bool> ToggleChecklistItemAsync(int itemId, int userId)
        {
            var item = await _context.ChecklistItems
                .Include(c => c.TravelPlan)
                .FirstOrDefaultAsync(c => c.Id == itemId && c.TravelPlan.UserId == userId);

            if (item == null) return false;

            item.IsCompleted = !item.IsCompleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChecklistItemAsync(int itemId, int userId)
        {
            var item = await _context.ChecklistItems
                .Include(c => c.TravelPlan)
                .FirstOrDefaultAsync(c => c.Id == itemId && c.TravelPlan.UserId == userId);

            if (item == null) return false;

            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChecklistItemResponseDto?> AddChecklistItemPublicAsync(int planId, CreateChecklistItemDto dto)
        {
            var plan = await _context.TravelPlans.FirstOrDefaultAsync(t => t.Id == planId);
            if (plan == null) return null;

            var item = new ChecklistItem
            {
                TravelPlanId = planId,
                Name = dto.Name,
                IsCompleted = false
            };

            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();

            return new ChecklistItemResponseDto
            {
                Id = item.Id,
                TravelPlanId = item.TravelPlanId,
                Name = item.Name,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<bool> ToggleChecklistItemPublicAsync(int itemId)
        {
            var item = await _context.ChecklistItems.FirstOrDefaultAsync(c => c.Id == itemId);
            if (item == null) return false;

            item.IsCompleted = !item.IsCompleted;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}