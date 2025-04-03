using Shared.Model.Base;

namespace Shared.Model.Entities
{
    public class SubscriptionPlan
    {
#nullable disable
        public short ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public short PlanType { get; set; }
        public short PlanDuration { get; set; }
        public string PlanId { get; set; }
        public decimal DisplayPrice { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime AddedOnUTC { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUTC { get; set; } = DateTime.UtcNow;
    }

   

}
