using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class LoginHistoryDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string UserName { get; set; } = string.Empty;
        public DateTime? LoginTime { get; set; }
        public DateTime? LogOutTime { get; set; }
        [Display(Name = "Login Time")]
        public string LoginDateString { get; set; } = string.Empty;
        [Display(Name = "Logout Time")]
        public string LogOutDateString { get; set; } = string.Empty;
        public int TotalRecord { get; set; }

    }
}
