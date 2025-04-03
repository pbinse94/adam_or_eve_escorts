using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.WebUser
{
    public class ContactUsRequestModel
    {
        [EmailAddress(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailValid", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailRequired", ErrorMessage = null)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "EmailLength", ErrorMessage = null)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "FirstNameRequired", ErrorMessage = null)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "LastNameRequired", ErrorMessage = null)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberRequired", ErrorMessage = null)]
        [StringLength(15, MinimumLength = 7, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "PhoneNumberLength", ErrorMessage = null)]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "QueryRequired", ErrorMessage = null)]
        [StringLength(600, MinimumLength = 15, ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "QueryLength", ErrorMessage = null)]
        public string Query { get; set; } = string.Empty;

        public string? CountryCode { get; set; }
    }
}
