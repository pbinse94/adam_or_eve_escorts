using Shared.Common.Enums;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.Account
{
    public class ApiLoginRequest
    {
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = $"EmailRequired", ErrorMessage = null)]
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PasswordRequired", ErrorMessage = null)]
        public string Password { get; set; } = string.Empty;
        public string DeviceToken { get; set; } = string.Empty;
        public short DeviceType { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public string DeviceLocation {get; set; } = string.Empty;
        public UserTypes? UserType { get; set; }
    }
}
