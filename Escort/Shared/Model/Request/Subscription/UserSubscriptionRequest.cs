namespace Shared.Model.Request.Subscription
{
    public class UserSubscriptionRequest
    {
#nullable disable
        public int UserId { get; set; }
        public byte SubscriptionId { get; set; }
        public string PaymentGatewaySubscriptionId { get; set; }
        public decimal Price { get; set; }
        public string CheckoutSessionId { get; set; }
        public short TransactionStatus { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
    }

    public class SubscriptionOrderResponse
    {
        public int UserSubscriptionId { get; set; }
        public string SubscriptionPlanPaypalId { get; set; }
    }
}
