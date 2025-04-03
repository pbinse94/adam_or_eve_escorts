using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.Account
{
    public class ForgetPasswordRequest
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;
    }
    public class ForgetPasswordTokenRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
