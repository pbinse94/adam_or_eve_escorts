using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Subscription;

namespace Data.IRepository
{
    public interface ISubscriptionPlanRepository : ICurdRepository<SubscriptionPlan>
    {
        Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetail(int userId);
        Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetailById(int userSubscriptionId);
        Task<SubscriptionPlan> GetSubscriptionDetailByPriceId(string priceId);
        Task<int> SaveUserSubscription(UserSubscriptionRequest request);
        Task<SubscriptionPlan> GetFreeSubscriptionPlanDetail();
        Task<int> UpdateUserSubscription(UserSubscriptionRequest request, int userSubscriptionId);
        Task<int> CancelUserSubscription(string paypalSubscriptionId, int userId);
        Task<int> DeactivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId, short status);
        Task<int> ActivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId);
        Task<int> SaveSubscriptionPaymentDetail(SaveSubscriptionPaymentDetailRequest request);
    }
}
