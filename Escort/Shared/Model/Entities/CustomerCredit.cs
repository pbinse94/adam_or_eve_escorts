using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities
{
    public class CreditPlan
    {
        #nullable disable
        public short ID { get; set; }
        public decimal CreditFrom { get; set; }
        public decimal Value { get; set; }
        public decimal CreditTo { get; set; }
        public short IsActive { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}