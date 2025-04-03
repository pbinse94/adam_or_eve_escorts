using Google.Apis.Auth.OAuth2.Responses;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface ICreditService
    {
        Task<List<CreditPlan>> GetCreditPlan();
        Task<ApiResponse<decimal>> CalculateCreditPrice(int creditQuantity);
        Task<ApiResponse<int>> SaveUserCredit(int creditQuantity, string token, int userId, decimal paidAmount);
        Task<ApiResponse<int>> UpdateCreditPaymentStatus(string transactionId, string paymentStatus, int userId);
    }
}
