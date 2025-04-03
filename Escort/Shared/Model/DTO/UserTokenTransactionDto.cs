using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class UserTokenTransactionDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        [Display(Name = "ID")]
        public int? UserId { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Escort Name")]
        public string? EscortName { get; set; }
        [Display(Name = "Transaction Type")]
        public string? Description { get; set; }

        [Display(Name = "Purchase ID")]
        public string? PurchaseId { get; set; }

        [Display(Name = "Points Purchased")]
        public decimal? PointsPurchased { get; set; }

        [Display(Name = "Points Spent")]
        public decimal? PointsSpent { get; set; }

        [Display(Name = "Points Balance")]
        public decimal? PointsBalance { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public int CurrentId { get; set; }

        public int FilterBy { get; set; }

        

        public int TotalRecord { get; set; }
        public int TransactionType { get; set; }
    }



    public class UserTokenTransactionClientDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; } 

        [Display(Name = "Date")]
        public DateTime Date { get; set; } 

        [Display(Name = "Points Purchased")]
        public decimal? PointsPurchased { get; set; }

        [Display(Name = "Points Spent")]
        public decimal? PointsSpent { get; set; }

        [Display(Name = "Points Balance")]
        public decimal? PointsBalance { get; set; } 


        public int TotalRecord { get; set; }
         
    }
}
