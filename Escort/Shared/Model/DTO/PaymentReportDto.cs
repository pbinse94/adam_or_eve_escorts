using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class PaymentReportDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        [Display(Name = "ID")]
        public int? UserId { get; set; }
        [Display(Name = "Escort ID")]
        public int? EscortId { get; set; }

        [Display(Name ="Is Payment Done")]
        public bool IsPaymentDone { get; set; }

        [Display(Name = "Escort Name")]
        public string? EscortName { get; set; }

        
        [Display(Name = "Account Holder Name")]
        public string? BankAccountHolderName { get; set; }

        [Display(Name = "Bank Name")]
        public string? BankName { get; set; }

        [Display(Name = "BSB Code")]
        public string? BsbNumber { get; set; }

        [Display(Name = "Account Number")]
        public string? BankAccountNumber { get; set; }

        [Display(Name = "Payment")]
        public decimal Payment { get; set; }


        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        
        public int PaidFilter { get; set; }
        public int TotalRecord { get; set; }
    }
}