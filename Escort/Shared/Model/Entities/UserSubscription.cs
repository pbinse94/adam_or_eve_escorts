using Shared.Model.Base;

namespace Shared.Model.Entities
{
    public class UserSubscription : BaseModel
    {
#nullable disable
        public int UserId { get; set; }
        public short SubscriptionId { get; set; }
        public decimal Price { get; set; }
        public string TransactionId { get; set; }
        public DateTime PurchaseDateUTC { get; set; }
        public DateTime ExpiryDateUTC { get; set; }
        public bool IsActive { get; set; }
        public bool IsCanceled { get; set; }
    }
}
