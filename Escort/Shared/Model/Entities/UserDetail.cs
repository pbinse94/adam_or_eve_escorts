using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{
    public class UserDetail : BaseModel
    {
#nullable disable
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; } 
        public string Email { get; set; }
        public short UserType { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Gender { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }

#nullable enable
        public bool? IsActive { get; set; }
        public bool IsApprove { get; set; }
        public string? ProfileImage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ResetPasswordToken { get; set; }
        public string? EmailVerifiedToken { get; set; }
        public string? AccessToken { get; set; }
        public string? ForgotPasswordDateUTC { get; set; }
        public short DeviceType { get; set; }
        public string? DeviceToken { get; set; }
        public string? UserStripeId { get; set; }
        public int TotalToken { get; set; }

 

    }
}
