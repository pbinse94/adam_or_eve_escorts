using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class SubscriptionTransactionsService : ISubscriptionTransactionsService
    {
        private readonly ISubscriptionTransactionsRepository _subscriptionTransactionsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubscriptionTransactionsService(ISubscriptionTransactionsRepository subscriptionTransactionsRepository,IHttpContextAccessor httpContextAccessor)
        {
            _subscriptionTransactionsRepository = subscriptionTransactionsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<SubscriptionTransactionsDto>>> GetSubscriptionTransactions(SubscriptionTransactionsRequestModel request)
        {
            ApiResponse<List<SubscriptionTransactionsDto>> response = new();
            var subscriptionTransactionsHistory = await _subscriptionTransactionsRepository.GetSubscriptionTransactions(request);

            if (subscriptionTransactionsHistory is null)
            {
                response.Data = new List<SubscriptionTransactionsDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            foreach(var u in subscriptionTransactionsHistory)
            {
                u.TransactionDateString = u.TransactionDate is null ? "N/A" : u.TransactionDate.Value.ToLocal(_httpContextAccessor);
                u.NextBillingTimeString = u.NextBillingTime is null ? "N/A" : u.NextBillingTime.Value.ToLocalDatePart(_httpContextAccessor);
                u.UserName = (u.UserName is null || u.UserName == " ") ? "N/A" : u.UserName;
                u.TransactionId = (u.TransactionId is null || u.TransactionId == " ") ? "N/A" : u.TransactionId;
                u.PaymentSubscriptionId = (u.PaymentSubscriptionId is null || u.PaymentSubscriptionId == " ") ? "N/A" : u.PaymentSubscriptionId;
            }

            response.Data = subscriptionTransactionsHistory;
            response.Message = ResourceString.Success;
            return response;

        }
    }
}
