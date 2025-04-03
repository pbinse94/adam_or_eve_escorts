using Business.Communication;
using Business.IServices;
using Data.IRepository;
using Shared.Model.Entities;
using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Common;
using static Google.Apis.Requests.BatchRequest;
using Shared.Model.Base;
using FirebaseAdmin.Messaging;
using Shared.Model.Response;
using Stripe.FinancialConnections;
using Shared.Model.DTO;
using Shared.Resources;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared.Model.Request.Subscription;
using Shared.Common.Enums;

namespace Business.Services
{
    public class CreditService : ICreditService
    {
        private readonly ICreditRepository _creditRepository;
        public CreditService(ICreditRepository creditRepository)
        {
            _creditRepository = creditRepository;
            
        }

        public async Task<List<CreditPlan>> GetCreditPlan()
        {
            var subscriptionPlans = await _creditRepository.GetAllAsync();
            return subscriptionPlans.ToList();  
        }


        public async Task<ApiResponse<decimal>> CalculateCreditPrice(int creditQuantity)
        {
            var subscriptionPlans = await _creditRepository.CalculateCreditPrice(creditQuantity);
            return new ApiResponse<decimal>(data: subscriptionPlans, message: ResourceString.Success);
        }


        public async Task<ApiResponse<int>> SaveUserCredit(int creditQuantity, string token, int userId, decimal paidAmount)
        {
            var subscriptionPlans = await _creditRepository.SaveUserCredit(creditQuantity, token, userId, paidAmount);
            return new ApiResponse<int>(data: subscriptionPlans, message: ResourceString.Success);
        }

        public async Task<ApiResponse<int>> UpdateCreditPaymentStatus(string transactionId, string paymentStatus, int userId)
        {
            var subscriptionPlans = await _creditRepository.UpdateCreditPaymentStatus(transactionId, paymentStatus, userId);
            return new ApiResponse<int>(data: subscriptionPlans, message: ResourceString.Success);
        }

    }
}