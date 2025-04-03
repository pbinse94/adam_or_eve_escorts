using Shared.Common;

namespace Shared.Model.Request.AdminUser
{
    public class ActivityLogHistoryRequestModel : DataTableParameters
    {
        public int AdminUserId { get; set; }

        public string? FromDate { get; set; }

        public string? ToDate { get; set; }
    }

    public class SubscriptionTransactionsRequestModel : DataTableParameters
    {
        public string? SearchKeyword { get; set; }
        public string? FromDate { get; set; }

        public string? ToDate { get; set; }
    }

}
