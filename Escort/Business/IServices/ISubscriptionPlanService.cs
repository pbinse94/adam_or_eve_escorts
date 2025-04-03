using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Subscription;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface ISubscriptionPlanService
    {
        Task<List<SubscriptionPlan>> GetSubscriptionPlansByPlanType(short planType);
        Task<SubscriptionPlan> GetSubscriptionPlanById(short planId);
        Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetailById(int userSubscriptionId);
        Task<ApiResponse<StripePaymentLinkResponse>> GetStripePaymentLink(string priceId, string userEmail);
    
        Task<ApiResponse<SubscriptionOrderResponse>> InitiateUserSubscription(int? userId, TransactionPaymentStatus paymentStatus, byte planId);
        Task<ApiResponse<ProfileDto>> SaveUserSubscriptionPaypal(string subscriptionId);
        Task<ApiResponse<GetUserSubscriptionDetailDto>> CompleteUserSubscription(SubscriptionDetails paypalObject, UserDetailsDto userDetail, TransactionPaymentStatus paymentStatus, GetUserSubscriptionDetailDto dbSubscription);
        //Task<ApiResponse<UserDetailsDto>> SaveUserSubscription(string sessionId);
        Task<int> CancelUserSubscriptionByWebhook(int userSubscriptionId, string paypalSubscriptionId, int userId);
        Task<int> DeactivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId, short status);
        Task<int> ActivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId);
        Task<ApiResponse<bool>> CancelUserSubscriptionByUser(int userId, byte planId);
        Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetail(int userId);
        Task<int> SaveSubscriptionPaymentDetail(SaveSubscriptionPaymentDetailRequest request);


    }
}
