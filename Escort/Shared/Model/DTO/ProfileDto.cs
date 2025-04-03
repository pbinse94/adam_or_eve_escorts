namespace Shared.Model.DTO
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImage { get; set; }
        public string? AuthorizationToken { get; set; }
        public string? AccessToken { get; set; }
        public short UserType { get; set; }
        public short PlanType { get; set; }
        public short PlanDuration { get; set; }
        public short SubscriptionPlanId { get; set; }
        
        public DateTime SubscriptionPlanExpireDateTime { get; set; }
        public string SubscriptionPaypalId { get; set; } = string.Empty;
        public string SubscriptionPlanPaypalId { get; set; } = string.Empty;
        public int CreditBalance { get; set; }
        public string? IpAddress { get; set; } 
    }
}
