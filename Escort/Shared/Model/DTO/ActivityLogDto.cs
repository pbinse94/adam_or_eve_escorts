using Shared.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class ActivityLogDto
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string AdminUserName { get; set; } = string.Empty;
        public DateTime? ActionDate { get; set; }
        [Display(Name = "Activity On")]
        public string ActionDateString { get; set; } = string.Empty;
        [Display(Name = "Description")]
        public string ActionDescription { get; set; } = string.Empty;
        public int TotalRecord { get; set; }
    }

    public class ActivityUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }

    public class ActivityLogRequestModel
    {
        public ActivityUser LoggedInUser { get; set; } = new();
        public ActivityUser? TargetUser { get; set; } = new();
        public int TargetId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string DbEntity { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
