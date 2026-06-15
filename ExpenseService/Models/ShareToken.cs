namespace ExpenseService.Models
{
    public class ShareToken
    {
        public int Id { get; set; }
        public int TravelPlanId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string AccessType { get; set; } = "VIEW";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}