using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Request.Admin
{
    public class EscortPaymentReportRequestModel : DataTableParameters
    {
        public string EscortName { get; set; }=string.Empty;
        public int EscortId { get; set; }
        public bool IsPaid { get; set; }
        public decimal AdminPercentage { get; set; }
    }
}
