using Shared.Common;

namespace Shared.Model.Request.AdminUser
{
    public class LoginHistoryRequestModel : DataTableParameters
    {
        public int UserId { get; set; }

        public string? FromDate { get; set; }

        public string? ToDate { get; set; }
    }
}
