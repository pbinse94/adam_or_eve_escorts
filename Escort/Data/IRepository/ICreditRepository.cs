using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Subscription;

namespace Data.IRepository
{
    public interface ICreditRepository : ICurdRepository<CreditPlan>
    {
        
        Task<decimal> CalculateCreditPrice(int creditQuantity);
        Task<int> SaveUserCredit(int creditQuantity, string token, int userId, decimal paidAmount);
        Task<int> UpdateCreditPaymentStatus(string transactionId, string paymentStatus, int userId);
    }
}