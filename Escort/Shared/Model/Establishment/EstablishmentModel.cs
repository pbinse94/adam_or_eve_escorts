using Shared.Model.DTO;
using Shared.Model.Escort;
using System.ComponentModel.DataAnnotations;
 
namespace Shared.Model.Establishment
{
    public class EstablishmentModel
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Country { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public bool? Subscription { get; set; }

        [Display(Name = "Company Name")]
        public string DisplayName { get; set; } = string.Empty;

        //public string? Status { get; set; }

        [Display(Name = "Status")]
        public bool? IsActive { get; set; }

        [Display(Name = "Action")]
        public string? Action { get; set; }

        public int TotalRecord { get; set; }

        public string? AccessToken { get; set; }


        public DateTime AddedOnUTC { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUTC { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; }


    }


    public class EstablishmentProfileModel 
    {
        public EscortDetailDto? EscortDetailDtoModel { get; set; }
        public EscortModel? EscortEstablishmentModel { get; set; }
    }



 }
