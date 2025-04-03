using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class UsersDto
    {
        [Display(Name = "S.No.")]
        public long Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        
        public String? Role { get; set; }

        //[Display(Name = "Display Name")]
        //public string? DisplayName { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        public int TotalRecord { get; set; }

        [Display(Name = "Register Date")]
        public string? RegisterDate { get; set; }

        [Display(Name = "Action")]
        public string? Action { get; set; }

       

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
