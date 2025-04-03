using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Entities
{
    public class BankDetails
    {
        public int EscortID { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "BankAccountHolderNameRequired", ErrorMessage = null)]
        public string? BankAccountHolderName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "BankAccountNumberRequired", ErrorMessage = null)]
        public string? BankAccountNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "BSBNumberRequired", ErrorMessage = null)]
        public string? BSBNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(ResourceString), ErrorMessageResourceName = "BankNameRequired", ErrorMessage = null)]
        public string? BankName { get; set; }



    }
}
