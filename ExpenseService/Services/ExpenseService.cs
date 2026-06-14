using Microsoft.EntityFrameworkCore;
using ExpenseService.Data;
using ExpenseService.DTOs;
using ExpenseService.Models;
using QRCoder;

namespace ExpenseService.Services
{
    public class ExpensePlanService
    {
        private readonly ExpenseDbContext _context;

        public ExpensePlanService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseResponseDto>> GetAllAsync(int travelPlanId, int userId)
        {
            return await _context.Expenses
                .Where(e => e.TravelPlanId == travelPlanId && e.UserId == userId)
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto, int userId)
        {
            var expense = new Expense
            {
                TravelPlanId = dto.TravelPlanId,
                UserId = userId,
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Date = dto.Date,
                Description = dto.Description
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return MapToDto(expense);
        }

        public async Task<ExpenseResponseDto?> UpdateAsync(int id, CreateExpenseDto dto, int userId)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null) return null;

            expense.Name = dto.Name;
            expense.Category = dto.Category;
            expense.Amount = dto.Amount;
            expense.Date = dto.Date;
            expense.Description = dto.Description;

            await _context.SaveChangesAsync();
            return MapToDto(expense);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int travelPlanId, int userId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.TravelPlanId == travelPlanId && e.UserId == userId)
                .ToListAsync();

            return new BudgetSummaryDto
            {
                TravelPlanId = travelPlanId,
                TotalExpenses = expenses.Sum(e => e.Amount),
                ByCategory = expenses
                    .GroupBy(e => e.Category)
                    .Select(g => new CategorySummaryDto
                    {
                        Category = g.Key,
                        Total = g.Sum(e => e.Amount)
                    }).ToList()
            };
        }

        public async Task<ShareTokenResponseDto> CreateShareTokenAsync(CreateShareTokenDto dto)
        {
            var token = Guid.NewGuid().ToString("N");
            var shareToken = new ShareToken
            {
                TravelPlanId = dto.TravelPlanId,
                Token = token,
                AccessType = dto.AccessType,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.ShareTokens.Add(shareToken);
            await _context.SaveChangesAsync();

            var shareUrl = $"http://localhost:3000/shared/{token}";
            var qrCodeBase64 = GenerateQrCode(shareUrl);

            return new ShareTokenResponseDto
            {
                Token = token,
                AccessType = dto.AccessType,
                ExpiresAt = shareToken.ExpiresAt,
                QrCodeBase64 = qrCodeBase64
            };
        }

        public async Task<ShareToken?> ValidateTokenAsync(string token)
        {
            var shareToken = await _context.ShareTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);

            return shareToken;
        }

        private static string GenerateQrCode(string url)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(10);
            return Convert.ToBase64String(qrBytes);
        }

        private static ExpenseResponseDto MapToDto(Expense e) => new()
        {
            Id = e.Id,
            TravelPlanId = e.TravelPlanId,
            UserId = e.UserId,
            Name = e.Name,
            Category = e.Category,
            Amount = e.Amount,
            Date = e.Date,
            Description = e.Description,
            CreatedAt = e.CreatedAt
        };
    }
}