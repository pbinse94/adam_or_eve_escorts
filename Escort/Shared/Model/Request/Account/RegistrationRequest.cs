using Shared.Common.Enums;
using Shared.Resources;
using Shared.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.Account
{
    public class RegistrationRequest
    {
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailLength", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "FirstNameRequired", ErrorMessage = null)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "LastNameRequired", ErrorMessage = null)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "DisplayNameRequired", ErrorMessage = null)]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessage = null)]
        [StringLength(15, MinimumLength = 7, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberLength", ErrorMessage = null)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "GenderRequired", ErrorMessage = null)]
        public string? Gender { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordRequired", ErrorMessage = null)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#\s]{6,20}$", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordValid", ErrorMessage = null)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessage = null)]
        [Compare("Password", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordValid", ErrorMessage = null)]
        public string ConfirmPassword { get; set; } = string.Empty;

        //[Required]
        //[MustBeTrue(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "AcceptTermAndConditons", ErrorMessage = null)]
        public bool TermsAndCondtion { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "AccountTypeRequired", ErrorMessage = null)]
        [DisplayName("Account Type")]
        public UserTypes UserType { get; set; }
       
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }
        public bool IsEmailVerified { get; set; }
        public string DeviceToken { get; set; } = string.Empty; 
        public string IpAddress { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public string DeviceLocation { get; set; } = string.Empty;
    }

    public class UserVerificationCode
    {
        public bool IsVerify { get; set; }  
        public string? Email { get; set; }
        public string? Code { get; set;}
    }
}
