using Shared.Model.DTO;
using Shared.Model.Request.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface ISubscriptionTransactionsRepository
    {
        Task<List<SubscriptionTransactionsDto>> GetSubscriptionTransactions(SubscriptionTransactionsRequestModel request);
    }
}
