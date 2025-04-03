using Shared.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class SubscriptionTransactionsDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string UserName { get; set; } = string.Empty;
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; } = string.Empty;
        [Display(Name = "Plan Duration")]
        public string? PlanDuration { get; set; }
        public DateTime? TransactionDate { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDateString { get; set; } = string.Empty;

        [Display(Name = "Subscription Id")]
        public string PaymentSubscriptionId { get; set; } = string.Empty;
        [Display(Name = "Transaction Id")]
        public string TransactionId { get; set; } = string.Empty;
        [Display(Name = "Transaction Status")]
        public string TransactionStatus { get; set; } = string.Empty;
        [Display(Name = "Status")]
        public string RenewalStatus { get; set; } = string.Empty;
        public DateTime? NextBillingTime { get; set; }
        [Display(Name = "Next Billing Date")]
        public string NextBillingTimeString { get; set; } = string.Empty;
        [Display(Name = "Paypal Fee($)")]
        public decimal TransactionFee { get; set; }
        [Display(Name = "Subscription Amount($)")]
        public decimal TransactionAmount { get; set; } 
        
        public int TotalRecord { get; set; }
    }
}
