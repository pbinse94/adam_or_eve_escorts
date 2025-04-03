using Microsoft.AspNetCore.Http;
using Shared.Common.Enums;
using Shared.Model.Entities;
using Shared.Model.Request.Profile;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class EscortDetailDto
    {
#nullable disable
        public int EscortID { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "FirstNameRequired", ErrorMessage = null)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "LastNameRequired", ErrorMessage = null)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "DisplayNameRequired", ErrorMessage = null)]
        public string DisplayName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailLength", ErrorMessage = null)]
        public string Email { get; set; }
#nullable enable
        public int? Age { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "GenderRequired", ErrorMessage = null)]
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public int? Height { get; set; }
        public string? BodyType { get; set; }
        public string? BankAccountHolderName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BSBNumber { get; set; }
        public string? BankName { get; set; }
        public string? Eyes { get; set; }
        public string[]? SexualPreferences { get; set; }
        public string[]? Categories{ get; set; }
        public string[]? Category { get; set; }
        public string[]? Language { get; set; }

        public string? SexualPreferencesId { get; set; }
        public string? CategoriesId { get; set; } 
        public string? CategoryId { get; set; }
        public string? LanguageId { get; set; }

        public IFormFile? ProfileFile { get; set; }
        public string? CroppedProfileFile { get; set; }
        public string? ProfileImage { get; set; }
        public IList<IFormFile>? Images { get; set; }
        public string? ImageUrls { get; set; }
        public IFormFile? Videos { get; set; }
        public string? VideoUrls { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?facebook\.com\/.*$",
      ErrorMessage = "Invalid Facebook URL.")]
        public string? FaceBookUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?twitter\.com\/.*$",
        ErrorMessage = "Invalid Twitter URL.")]
        public string? TwitterUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?instagram\.com\/.*$",
        ErrorMessage = "Invalid Instagram URL.")]

        public string? InstagramUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?linkedin\.com\/.*$",
        ErrorMessage = "Invalid LinkedIn URL.")]
        public string? LinkedinUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?tiktok\.com\/@.*$",
       ErrorMessage = "Invalid TikTok URL.")]
        public string? TikTokUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?snapchat\.com\/add\/.*$",
        ErrorMessage = "Invalid Snapchat URL.")]
        public string? SnapChatUrl { get; set; }
        [RegularExpression(@"^(https?:\/\/)?(www\.)?youtube\.com\/.*$",
       ErrorMessage = "Invalid YouTube URL.")]
        public string? YouTubeUrl { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? Proffered { get; set; }

        public string? Ethnicity { get; set; }
        public string? Weight { get; set; }
        public string? Bust { get; set; }
        public string? Dress { get; set; }
        public string? HairColor { get; set; }
        public string? RatesNotes { get; set; }
        public string? AvailablityNotes { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessage = null)]
        [StringLength(15, MinimumLength = 7, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberLength", ErrorMessage = null)]
        public string? PhoneNumber { get; set; }

        public List<EscortRatesDto> Rates { get; set; } = new List<EscortRatesDto>();
        public List<AvailabilityCalendarDto> AvailabilityCalendar { get; set; } = new List<AvailabilityCalendarDto>();
        public List<LocationDto> TourLocation { get; set; } = new List<LocationDto>();
        public List<LocationDto> BaseLocation { get; set; } = new List<LocationDto>();
        public List<EscortGalleryDto> EscortGallery { get; set; } = new List<EscortGalleryDto>();
        public bool? IsActive { get; set; }
        public bool IsPhotoVerified { get; set; } = false;
        public int UserType { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public EmailRequestModel EmailRequestModel { get; set; } = new EmailRequestModel();
        public EscortSubscriptionDto? EscortSubscription { get; set; }
        public string CategoriesNames { get; set; } = string.Empty;
        public int TotalViews { get; set; }
        public int TotalFavoriteMarkByUsers { get; set; }
        public bool? IsPaused { get; set; }
        public bool IsApprove { get; set; }
    }

    public class EscortSubscriptionDto
    {
        public SubscriptionPlanDurationType SubscriptionPlanId { get; set; }
        public UserTypes UserType { get; set; }
        public DateTime? PurchaseDateUTC { get; set; }
        public DateTime? ExpiryDateUTC { get; set; }

        public string PurchaseDateString { get; set; } = string.Empty;
        public string ExpiryDateString { get; set; } = string.Empty;
    }

}
