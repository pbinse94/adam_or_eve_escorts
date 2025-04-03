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
using System.Numerics;
using Azure.Core;
using Data.Repository;

namespace Business.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStripeClient _stripeClient;
        private readonly IEmailFunctions _emailFunctions;
        private readonly PayPalService _payPalService;
        public SubscriptionPlanService(ISubscriptionPlanRepository subscriptionPlanRepository, IAccountRepository accountRepository, IEmailFunctions emailFunctions, PayPalService payPalService)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _stripeClient = new StripeClient(SiteKeys.StripeSecreatKey);
            _accountRepository = accountRepository;
            _emailFunctions = emailFunctions;
            _payPalService = payPalService;
        }

        public async Task<List<SubscriptionPlan>> GetSubscriptionPlansByPlanType(short planType)
        {
            var subscriptionPlans = await _subscriptionPlanRepository.GetAllAsync();
            return subscriptionPlans.Where(x => x.PlanType == planType).ToList();
        }

        public async Task<SubscriptionPlan> GetSubscriptionPlanById(short planId)
        {
            return await _subscriptionPlanRepository.GetByIdAsync(planId);
        }
        public async Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetailById(int userSubscriptionId)
        {
            return await _subscriptionPlanRepository.GetUserSubscriptionDetailById(userSubscriptionId);
        }

        public async Task<SubscriptionPlan> GetUserSubscriptionPlanById(short planId)
        {
            return await _subscriptionPlanRepository.GetByIdAsync(planId);
        }

        public async Task<ApiResponse<StripePaymentLinkResponse>> GetStripePaymentLink(string priceId, string userEmail)
        {
            // Create session options
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                SuccessUrl = SiteKeys.SiteUrl + "Account/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = SiteKeys.SiteUrl + "Account/Cancel?session_id={CHECKOUT_SESSION_ID}",
                Mode = "subscription",
                CustomerEmail = userEmail,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    }
                }
            };

            var service = new Stripe.Checkout.SessionService(_stripeClient);
            try
            {
                var session = await service.CreateAsync(options);
                return new ApiResponse<StripePaymentLinkResponse>(new StripePaymentLinkResponse() { SessionId = session.Id, Url = session.Url });
            }
            catch (StripeException ex)
            {
                return new ApiResponse<StripePaymentLinkResponse>(data: null, message: ex.StripeError.Message);
            }
        }



        //public async Task<ApiResponse<UserDetailsDto>> SaveUserSubscription(string sessionId)
        //{

        //    var service = new Stripe.Checkout.SessionService(_stripeClient);
        //    var sessionObject = await service.GetAsync(sessionId);

        //    if (string.IsNullOrEmpty(sessionObject.CustomerEmail))
        //    {
        //        return new ApiResponse<UserDetailsDto>(data: null, message: ResourceString.SomethingWrong);
        //    }

        //    var userDetail = await _accountRepository.FindByEmailAsync(sessionObject.CustomerEmail);

        //    if (userDetail == null)
        //    {
        //        return new ApiResponse<UserDetailsDto>(data: null, message: ResourceString.SomethingWrong);
        //    }

        //    TransactionPaymentStatus paymentStatus = sessionObject.PaymentStatus != "unpaid" ? TransactionPaymentStatus.Success : TransactionPaymentStatus.Canceled;
        //    if (sessionObject.PaymentStatus != "unpaid")
        //    {
        //        int userUpdated = await _accountRepository.AddUpdateAsync(new UserDetail() { UserStripeId = sessionObject.CustomerId, Id = userDetail.UserId });
        //        if (userUpdated > 0)
        //        {
        //            userDetail.UserStripeId = sessionObject.CustomerId;
        //        }
        //    }


        //    ApiResponse<SubscriptionPlan> result = await SaveUserSubscription(sessionObject.Id, sessionObject.SubscriptionId, userDetail, paymentStatus);
        //    if (result.Data != null)
        //    {
        //        userDetail.SubscriptionPlanDurationType = (SubscriptionPlanDurationType)result.Data.PlanDuration;
        //        userDetail.SubscriptionPlanType = (SubscriptionPlanType)result.Data.PlanType;
        //        return new ApiResponse<UserDetailsDto>(data: userDetail);
        //    }
        //    else
        //    {
        //        return new ApiResponse<UserDetailsDto>(data: null);
        //    }

        //    //await _subscriptionService.CancelSubscription(UserSession.userId, false, "");
        //}

        public async Task<GetUserSubscriptionDetailDto> GetUserSubscriptionDetail(int userId)
        {
            return await _subscriptionPlanRepository.GetUserSubscriptionDetail(userId);

        }

    public async Task<ApiResponse<ProfileDto>> SaveUserSubscriptionPaypal(string subscriptionId)
        {

            // Fetch subscription details from PayPal
            var paypalObject = await _payPalService.GetSubscriptionDetailsAsync(subscriptionId);

            // Check if the PayPal object is null (error case)
            if (paypalObject == null)
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.SomethingWrong);
            }

            if (string.IsNullOrEmpty(paypalObject.CustomId))
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.SomethingWrong);
            }

            int userSubscriptionId = Convert.ToInt32(paypalObject.CustomId.ToDecrypt());

            if (userSubscriptionId <= 0)
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.SomethingWrong);
            }

            var userSubscription = await GetUserSubscriptionDetailById(userSubscriptionId);
            if (userSubscription == null)
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.SomethingWrong);
            }

            // Find user details by email
            var userDetail = await _accountRepository.FindByEmailAsync(userSubscription.Email);

            // Check if user details were found
            if (userDetail == null)
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.UserDetailsNotFound);
            }

            // Determine payment status based on PayPal object
            TransactionPaymentStatus paymentStatus = paypalObject.Status == PaypalSubscriptionStatus.ACTIVE.ToString()
                ? TransactionPaymentStatus.Success
                : TransactionPaymentStatus.Cancelled;



            // Save user subscription
            var result = await CompleteUserSubscription(paypalObject, userDetail, paymentStatus, userSubscription);
            if (result.Data != null)
            {
                var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(userDetail.UserId);

                ProfileDto profileDto = new ProfileDto
                {
                    UserId = userDetail.UserId,
                    FirstName = userDetail.FirstName,
                    LastName = userDetail.LastName,
                    DisplayName = userDetail.DisplayName,
                    Email = userDetail.Email,
                    ProfileImage = userDetail.ProfileImage,
                    AccessToken = userDetail.AccessToken,
                    UserType = userDetail.UserType
                };

                if (activeSubscription != null)
                {
                    profileDto.PlanType = activeSubscription.PlanType;
                    profileDto.PlanDuration = activeSubscription.PlanDuration;
                    profileDto.SubscriptionPlanId = activeSubscription.SubscriptionId;
                    profileDto.SubscriptionPlanExpireDateTime = activeSubscription.ExpiryDateUTC;
                }

                return new ApiResponse<ProfileDto>(data: profileDto);
            }
            else
            {
                return new ApiResponse<ProfileDto>(data: null, message: ResourceString.SomethingWrong);
            }
        }


        //public async Task<ApiResponse<SubscriptionPlan>> SaveUserSubscription(string checkoutSessionId, string subscriptionId, UserDetailsDto userDetail, TransactionPaymentStatus paymentStatus)
        //{
        //    StripeUtility stripUtility = new StripeUtility();

        //    var checkSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(userDetail.UserId);

        //    UserSubscriptionRequest userSubscriptionDetail = new UserSubscriptionRequest()
        //    {
        //        UserId = userDetail.UserId,
        //        CheckoutSessionId = checkoutSessionId,
        //        TransactionType = (byte)TransactionType.Subscription,
        //        TransactionStatus = (byte)paymentStatus,
        //    };

        //    Subscription? subscription = null;
        //    SubscriptionPlan? dbSubscription = null;

        //    if (!string.IsNullOrEmpty(subscriptionId))
        //    {
        //        var subscriptionApiResult = await stripUtility.GetSubscriptionAsync(subscriptionId);

        //        if (subscriptionApiResult == null || subscriptionApiResult.Data == null)
        //        {
        //            return new ApiResponse<SubscriptionPlan>(dbSubscription, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
        //        }

        //        subscription = subscriptionApiResult.Data;

        //        dbSubscription = await _subscriptionPlanRepository.GetSubscriptionDetailByPriceId(subscription.Items?.Data[0]?.Price.Id ?? string.Empty);

        //        userSubscriptionDetail.SubscriptionId = (byte)dbSubscription.ID;
        //        userSubscriptionDetail.PaymentGatewaySubscriptionId = subscription.Id;
        //        userSubscriptionDetail.PurchaseDate = subscription.CurrentPeriodStart;
        //        userSubscriptionDetail.ExpiryDate = subscription.CurrentPeriodEnd;
        //        userSubscriptionDetail.Price = (subscription.Items?.Data[0]?.Price.UnitAmountDecimal ?? 0) / 100;

        //    }

        //    var result = await _subscriptionPlanRepository.SaveUserSubscription(userSubscriptionDetail);

        //    if (subscription != null && dbSubscription != null && subscription.CurrentPeriodEnd > DateTime.UtcNow && subscription.Status == "active" && subscription.CurrentPeriodEnd != DateTime.MinValue)
        //    {

        //        if (result > 0)
        //        {
        //            string purchaseDate = userSubscriptionDetail.PurchaseDate?.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
        //            string expiryDate = userSubscriptionDetail.ExpiryDate?.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
        //            string subscriptionTypeString = $"${dbSubscription.Title} ${Enum.GetName(typeof(SubscriptionPlanDurationType), dbSubscription.PlanType)} Plan";

        //            if (checkSubscription == null)
        //            {
        //                await _emailFunctions.PlanPurchasedSuccessMail(userDetail.Email, ResourceString.PlanPurchased, userDetail.DisplayName, subscriptionTypeString, purchaseDate, expiryDate);
        //            }
        //            else
        //            {
        //                await _emailFunctions.PlanChangedSuccessMail(userDetail.Email, ResourceString.PlanChanged, userDetail.DisplayName, subscriptionTypeString, purchaseDate, expiryDate);
        //            }
        //            return new ApiResponse<SubscriptionPlan>(dbSubscription, message: ResourceString.PurchaseSubscription, apiName: "SaveUserSubscription");
        //        }
        //        else
        //        {
        //            return new ApiResponse<SubscriptionPlan>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");

        //        }
        //    }
        //    return new ApiResponse<SubscriptionPlan>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
        //}

        public async Task<ApiResponse<bool>> CancelUserSubscriptionByUser(int userId, byte planId)
        {
            var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(userId);


            if (activeSubscription == null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "CancelUserSubscription");
            }

            bool status = false;
            if (activeSubscription.SubscriptionId == (short)SubscriptionPlanDurationType.IndependentEscortBasic)
            {
                status = true;
            }
            else
            {
                status = await _payPalService.CancelSubscriptionAsync(activeSubscription.PaymentSubscriptionId);
            }

            if (status)
            {
                int result = await _subscriptionPlanRepository.CancelUserSubscription(activeSubscription.PaymentSubscriptionId, userId);
                if (result > 0)
                {
                    return new ApiResponse<bool>(true, message: ResourceString.SubscriptionCancelledSuccess, apiName: "CancelUserSubscription");
                }
            }

            return new ApiResponse<bool>(false, message: ResourceString.SubscriptionCancelledError, apiName: "CancelUserSubscription");

        }


        public async Task<ApiResponse<SubscriptionOrderResponse>> InitiateUserSubscription(int? userId, TransactionPaymentStatus paymentStatus, byte planId)
        {
            SubscriptionPlan? dbSubscription = null;
            dbSubscription = await _subscriptionPlanRepository.GetByIdAsync(planId);

            if (dbSubscription == null)
            {
                return new ApiResponse<SubscriptionOrderResponse>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
            }


            UserSubscriptionRequest userSubscriptionDetail = new UserSubscriptionRequest()
            {
                UserId = userId ?? 0,
                TransactionStatus = (byte)paymentStatus,
                SubscriptionId = planId
            };

            userSubscriptionDetail.Price = dbSubscription.DisplayPrice;


            var userSubscriptionId = await _subscriptionPlanRepository.SaveUserSubscription(userSubscriptionDetail);

            if (userSubscriptionId <= 0)
            {
                return new ApiResponse<SubscriptionOrderResponse>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
            }

            SubscriptionOrderResponse result = new SubscriptionOrderResponse()
            {
                UserSubscriptionId = userSubscriptionId,
                SubscriptionPlanPaypalId = dbSubscription.PlanId
            };

            return new ApiResponse<SubscriptionOrderResponse>(result, message: ResourceString.PurchaseSubscription, apiName: "SaveUserSubscription");
        }

        public async Task<ApiResponse<GetUserSubscriptionDetailDto>> CompleteUserSubscription(SubscriptionDetails paypalObject, UserDetailsDto userDetail, TransactionPaymentStatus paymentStatus, GetUserSubscriptionDetailDto dbSubscription)
        {


            UserSubscriptionRequest userSubscriptionDetail = new UserSubscriptionRequest()
            {
                UserId = userDetail.UserId,
                TransactionStatus = (byte)paymentStatus,
            };


            userSubscriptionDetail.SubscriptionId = (byte)dbSubscription.SubscriptionId;
            userSubscriptionDetail.PaymentGatewaySubscriptionId = paypalObject.Id;
            userSubscriptionDetail.PurchaseDate = paypalObject.StartTime;
            userSubscriptionDetail.ExpiryDate = paypalObject.BillingInfo?.NextBillingTime ?? null;
            userSubscriptionDetail.Price = Convert.ToDecimal(paypalObject.BillingInfo?.LastPayment?.Amount?.Value);


            var result = await _subscriptionPlanRepository.UpdateUserSubscription(userSubscriptionDetail, Convert.ToInt32(paypalObject.CustomId.ToDecrypt()));

            if (paypalObject.BillingInfo != null && paypalObject.BillingInfo?.NextBillingTime != DateTime.MinValue && paypalObject.BillingInfo?.NextBillingTime > DateTime.UtcNow && paypalObject.Status == PaypalSubscriptionStatus.ACTIVE.ToString())
            {

                if (result > 0)
                {
                    //string purchaseDate = userSubscriptionDetail.PurchaseDate?.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
                    //string expiryDate = userSubscriptionDetail.ExpiryDate?.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
                    //string subscriptionTypeString = $"${dbSubscription.Title} ${Enum.GetName(typeof(SubscriptionPlanDurationType), dbSubscription.PlanType)} Plan";

                    //if (checkSubscription == null)
                    //{
                    //    await _emailFunctions.PlanPurchasedSuccessMail(userDetail.Email, ResourceString.PlanPurchased, userDetail.DisplayName, subscriptionTypeString, purchaseDate, expiryDate);
                    //}
                    //else
                    //{
                    //    await _emailFunctions.PlanChangedSuccessMail(userDetail.Email, ResourceString.PlanChanged, userDetail.DisplayName, subscriptionTypeString, purchaseDate, expiryDate);
                    //}
                    return new ApiResponse<GetUserSubscriptionDetailDto>(dbSubscription, message: ResourceString.PurchaseSubscription, apiName: "SaveUserSubscription");
                }
                else
                {
                    return new ApiResponse<GetUserSubscriptionDetailDto>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");

                }
            }
            return new ApiResponse<GetUserSubscriptionDetailDto>(null, message: ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
        }

        public async Task<int> CancelUserSubscriptionByWebhook(int userSubscriptionId, string paypalSubscriptionId, int userId)
        {
            return await _subscriptionPlanRepository.CancelUserSubscription(paypalSubscriptionId, userId);
        }

        public async Task<int> ActivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId)
        {
            return await _subscriptionPlanRepository.ActivateUserSubscription(userSubscriptionId, paypalSubscriptionId);
        }

        public async Task<int> DeactivateUserSubscription(int userSubscriptionId, string paypalSubscriptionId, short status)
        {
            return await _subscriptionPlanRepository.DeactivateUserSubscription(userSubscriptionId, paypalSubscriptionId, status);
        }

        public async Task<int> SaveSubscriptionPaymentDetail(SaveSubscriptionPaymentDetailRequest request)
        {
            return await _subscriptionPlanRepository.SaveSubscriptionPaymentDetail(request);
        }

    }
}
