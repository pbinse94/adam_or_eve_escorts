namespace Shared.Model.DTO
{
    public class GetUserSubscriptionDetailDto
    {
#nullable disable
        public int UserSubscriptionId { get; set; } 
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime PurchaseDateUTC { get; set; }
        public DateTime ExpiryDateUTC { get; set; }
        public short SubscriptionId { get; set; }
        
        public string PaymentSubscriptionId { get; set; }
        public string SubscriptionPlanPaypalId { get; set; }
        public short PlanType { get; set; }
        public short PlanDuration { get; set; }
        public bool IsActive { get; set; }
    }
}
