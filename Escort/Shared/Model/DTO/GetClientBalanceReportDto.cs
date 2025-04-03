using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{

    public class GetClientBalanceReportDto
    {
        public GetClientDetailDto? Client { get; set; }
        public List<GetClientTransactionReportDto>? Transactions { get; set; }
    }

    public class GetClientTransactionReportDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Escort Name")]
        public string UserDisplayName { get; set; } = string.Empty;

        [Display(Name = "Points Purchased")]
        public decimal PointsPurchased { get; set; }

        [Display(Name = "Amount")]
        public decimal PointsPurchasedAmount { get; set; }

        [Display(Name = "Points Spent")]
        public decimal PointsSpent { get; set; }

        [Display(Name = "Points Balance")]
        public decimal PointsBalance { get; set; }
        public short TransactionType { get; set; }

        [Display(Name = "Transaction Id")]
        public string TransactionId { get; set; } = string.Empty;
    }

    public class GetClientDetailDto
    {
        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
