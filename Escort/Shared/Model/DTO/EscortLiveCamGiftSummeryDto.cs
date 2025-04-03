using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class EscortLiveCamGiftSummeryDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        [Display(Name = "Date")]
        public DateTime LiveCamDate { get; set; }

        [Display(Name = "Clients")]
        public int LiveCamClients { get; set; }

        [Display(Name = "Credits")]
        public int TotalRecievedCredits { get; set; }

        [Display(Name = "Gifts")]
        public int TotalGiftCredits { get; set; }

        [Display(Name = "Revenue")]
        public decimal TotalRevenue { get; set; }
        public int TotalRecord { get; set; }
    }
}
