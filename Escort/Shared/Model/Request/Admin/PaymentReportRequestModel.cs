using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Request.Admin
{
    public class PaymentReportRequestModel : DataTableParameters
    {
        public string? FromDate { get; set; }
        
        public string? ToDate { get; set; }
        public bool IsMonthly {  get; set; }
        public bool IsWeekly {  get; set; }
        public bool IsPaid {  get; set; }
        public int? PaidFilter { get; set; }
    }
}