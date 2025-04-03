using Shared.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Escort
{
    public class EscortModel
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        public string? Name { get; set; }
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }


        public string? Email { get; set; }

        public string? Country { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public string? Category { get; set; }

        public short UserType { get; set; }

        public bool? Subscription { get; set; }
        [Display(Name = "Package Name")]
        public string PackageName { get; set; } = string.Empty;
        [Display(Name = "Client Views")]
        public int TotalViews { get; set; }

        //public string? Status { get; set; }

        [Display(Name = "Status")]
        public bool? IsActive { get; set; }

        [Display(Name = "Is Paused")]
        public bool IsPaused { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApprove { get; set; }


        [Display(Name = "Action")]
        public string? Action { get; set; }



        public int TotalRecord { get; set; }

        public string? AccessToken { get; set; }


        public DateTime AddedOnUTC { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUTC { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; }


    }
}
