using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class GetLastTwelveMonthSubscriptionReportDto
    {
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
