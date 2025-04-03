using Newtonsoft.Json;
using Shared.Resources;
using Shared.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.Account
{
    public class LoginRequest : ApiLoginRequest
    {        
        public string? ReturnUrl { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
    

    public class ForgotPasswordRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordRequired", ErrorMessage = null)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#\s]{6,20}$", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordValid", ErrorMessage = null)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessage = null)]
        [Compare("Password", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordValid", ErrorMessage = null)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool ValidToken { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "OldPasswordRequired", ErrorMessage = null)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "NewPasswordRequired", ErrorMessage = null)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#\s]{6,20}$", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordValid", ErrorMessage = null)]
        [NotEqual("OldPassword", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "OldNewPasswordNotSame", ErrorMessage = null)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessage = null)]
        [Compare("Password", ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "ConfirmPasswordValid", ErrorMessage = null)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class VerifyEmailRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;
    }
    public class CookieData
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        // Add other properties as needed
    }

    public class PartInfo
    {
        [JsonProperty("partNumber")]
        public int PartNumber { get; set; }
        [JsonProperty("eTag")]

        public string? ETag { get; set; }
    }


    public class VideoInfo
    {
        [JsonProperty("bucketName")]
        public string? BucketName { get; set; }
        [JsonProperty("objectKey")]
        public string? ObjectKey { get; set; }
        [JsonProperty("uploadId")]
        public string? UploadId { get; set; }
        [JsonProperty("partETags")]
        public  List<PartInfo>? PartETags { get; set; }
    }

    public class VerifyEmailCodeRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty; 
        public string Code { get; set; } = string.Empty;
    }
}
