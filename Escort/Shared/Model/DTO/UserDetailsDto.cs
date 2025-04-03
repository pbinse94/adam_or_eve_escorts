using Microsoft.AspNetCore.Http;
using Shared.Common.Enums;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class UserDetailsDto
    {
#nullable disable
        public int UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "NameRequired", ErrorMessage = null)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public short UserType { get; set; }
        public string PasswordHash { get; set; }
        public IFormFile ProfileFile { get; set; }
        public string CroppedProfileFile { get; set; }
        public string ProfileImage { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessage = null)]
        [StringLength(15, MinimumLength = 7, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberLength", ErrorMessage = null)]
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsApprove { get; set; }
        public bool IsEmailVerified { get; set; }
        public string AccessToken { get; set; }
        public int CreditBalance { get; set; }
        public string ResetToken { get; set; }
        public string EmailVerificationToken { get; set; }
        public string IpAddress { get; set; }
        public SubscriptionPlanType SubscriptionPlanType { get; set; }
        public SubscriptionPlanDurationType? SubscriptionPlanDurationType { get; set; }
        
    }

    public class LoginDevicInfo
    {
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string DeviceToken { get; set; }
        public string LogoutTime { get; set; }
    }
}
