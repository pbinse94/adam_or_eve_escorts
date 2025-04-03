namespace Shared.Model.Request.Subscription
{
    public class SaveSubscriptionPaymentDetailRequest
    {
        public int UserSubscriptionId { get; set; }
        public string VendorSubscriptionId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
        public decimal TransactionFee { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? TransactionDateTimeUTC { get; set; }
    }
}
