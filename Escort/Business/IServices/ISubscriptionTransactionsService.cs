using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Request.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.IServices
{
    public interface ISubscriptionTransactionsService
    {
        Task<ApiResponse<List<SubscriptionTransactionsDto>>> GetSubscriptionTransactions(SubscriptionTransactionsRequestModel request);
    }
}
