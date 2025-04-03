using Shared.Common;

namespace Shared.Model.Request.Admin
{
    public class UsersRequestModel : DataTableParameters
    {
        public int EstablishmentId { get; set; }
        public int UserType { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; } 
    }
}
