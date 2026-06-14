namespace ExpenseService.DTOs
{
    public class CreateExpenseDto
    {
        public int TravelPlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class BudgetSummaryDto
    {
        public int TravelPlanId { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<CategorySummaryDto> ByCategory { get; set; } = new();
    }

    public class CategorySummaryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class CreateShareTokenDto
    {
        public int TravelPlanId { get; set; }
        public string AccessType { get; set; } = "VIEW";
    }

    public class ShareTokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string AccessType { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}