using Azure.Core;
using Business.Communication;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Resources;
using Stripe;
using Web.Controllers.Base;

namespace Web.Controllers
{
    //[Route("subscription")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class SubscriptionController : UserBaseController
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly PayPalService _payPalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailFunctions _emailFunctions;
        private readonly IAccountService _accountService;
        public SubscriptionController(ISubscriptionPlanService subscriptionPlanService, PayPalService payPalService, IHttpContextAccessor httpContextAccessor, IEmailFunctions emailFunctions, IAccountService accountService)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _payPalService = payPalService;
            _httpContextAccessor = httpContextAccessor;
            _emailFunctions = emailFunctions;
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> GetSubscriptionPlan(short planDuration = (short)SubscriptionPlanDurationType.IndependentEscortBasic)
        {
            UserTypes userType = UserTypes.IndependentEscort;
            SubscriptionPlanType subscriptionPlanType = SubscriptionPlanType.Escort;
            if (LoginMemberSession.UserDetailSession?.UserTypeId != null)
            {
                bool success = Enum.IsDefined(typeof(Shared.Common.Enums.UserTypes), LoginMemberSession.UserDetailSession.UserTypeId);
                if (success)
                {
                    userType = (Shared.Common.Enums.UserTypes)LoginMemberSession.UserDetailSession.UserTypeId;
                }
            }

            if (userType == UserTypes.Establishment)
            {
                subscriptionPlanType = SubscriptionPlanType.Establishment;
            }
            var subscriptionPlans = await _subscriptionPlanService.GetSubscriptionPlansByPlanType((short)subscriptionPlanType);
            var orderedSubscriptionPlans = subscriptionPlans.OrderBy(plan => plan.DisplayPrice).ToList();

            var currentPlan = orderedSubscriptionPlans.FirstOrDefault(plan => plan.ID == LoginMemberSession.UserDetailSession?.SubscriptionPlanId);
            if (currentPlan != null)
            {
                ViewBag.CurrentPlanPrice = currentPlan.DisplayPrice;
            }
            else
            {
                ViewBag.CurrentPlanPrice = 0.00;
            }

            return PartialView("_SubscriptionPlans", new Tuple<List<SubscriptionPlan>, UserTypes>(orderedSubscriptionPlans, userType));
        }

        public async Task<IActionResult> Cancel(string subscription_id = "")
        {
            await _subscriptionPlanService.SaveUserSubscriptionPaypal(subscription_id);
            if (LoginMemberSession.UserDetailSession != null)
            {
                var loginMemberSession = LoginMemberSession.UserDetailSession;
                if (LoginMemberSession.UserDetailSession.UserTypeId == (int)UserTypes.IndependentEscort)
                {
                    await _accountService.SaveFreeSubscription(LoginMemberSession.UserDetailSession.EmailId ?? "");
                    var activeSubscription = await _subscriptionPlanService.GetUserSubscriptionDetail(LoginMemberSession.UserDetailSession?.UserId ?? 0);
                    if (activeSubscription != null)
                    {
                        loginMemberSession.SubscriptionPlanPaypalId = activeSubscription.SubscriptionPlanPaypalId;
                        loginMemberSession.SubscriptionPlanDurationType = (SubscriptionPlanDurationType)activeSubscription.PlanDuration;
                        loginMemberSession.SubscriptionPlanType = (SubscriptionPlanType)activeSubscription.PlanType;
                        loginMemberSession.SubscriptionPlanId = activeSubscription.SubscriptionId;
                        loginMemberSession.SubscriptionPlanExpireDateTime = activeSubscription.ExpiryDateUTC;
                        loginMemberSession.SubscriptionPlanExpireOn = activeSubscription.ExpiryDateUTC.ToLocal(_httpContextAccessor);
                    }
                }
                LoginMemberSession.UserDetailSession = loginMemberSession;
            }

            return View();
        }

        public async Task<IActionResult> Success(string subscription_id = "")
        {
            var userDetail = await _subscriptionPlanService.SaveUserSubscriptionPaypal(subscription_id);

            ViewBag.SubscriptionSuccess = false;

            if (userDetail == null || userDetail.Data == null)
            {
                ViewBag.Message = ResourceString.SubscriptionFailed;
            }
            else
            {
                ViewBag.SubscriptionSuccess = true;
                ViewBag.Message = ResourceString.SubscriptionSuccess;
                LoginSessionModel sessionobj = new LoginSessionModel()
                {
                    UserId = userDetail.Data.UserId,
                    FirstName = userDetail.Data.FirstName,
                    LastName = userDetail.Data.LastName,
                    DisplayName = userDetail.Data.DisplayName,
                    UserTypeId = userDetail.Data.UserType,
                    EmailId = userDetail.Data.Email,
                    SubscriptionPlanDurationType = (SubscriptionPlanDurationType)userDetail.Data.PlanDuration,
                    SubscriptionPlanType = (SubscriptionPlanType)userDetail.Data.PlanType,
                    SubscriptionPlanId = userDetail.Data.SubscriptionPlanId,
                    SubscriptionPlanExpireDateTime = userDetail.Data.SubscriptionPlanExpireDateTime,
                    SubscriptionPlanExpireOn = userDetail.Data.SubscriptionPlanExpireDateTime.ToLocal(_httpContextAccessor),
                    AccessToken = userDetail.Data.AccessToken
                };
                LoginMemberSession.UserDetailSession = sessionobj;
                string purchaseDate = System.DateTime.UtcNow.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
                string expiryDate = userDetail.Data.SubscriptionPlanExpireDateTime.ToLocal(_httpContextAccessor);
                string subscriptionTypeString = $"{EnumExtensions.GetDescription((SubscriptionPlanDurationType)userDetail.Data.SubscriptionPlanId)} Plan";

                await _emailFunctions.PlanPurchasedSuccessMail(userDetail.Data?.Email ?? "", ResourceString.PlanPurchased, userDetail.Data?.DisplayName ?? "", subscriptionTypeString, purchaseDate, expiryDate);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription(byte planId)
        {
            string returnUrl = SiteKeys.SiteUrl + "Subscription/Success";
            string cancelUrl = SiteKeys.SiteUrl + "Subscription/Cancel";

            var userSubscription = await _subscriptionPlanService.InitiateUserSubscription(LoginMemberSession.UserDetailSession?.UserId, planId == (byte)SubscriptionPlanDurationType.IndependentEscortBasic ? TransactionPaymentStatus.Success : TransactionPaymentStatus.Pending, planId);

            if (userSubscription == null || userSubscription.Data == null || string.IsNullOrEmpty(userSubscription.Data.SubscriptionPlanPaypalId))
            {

                var activeSubscription = await _subscriptionPlanService.GetUserSubscriptionDetail(LoginMemberSession.UserDetailSession?.UserId ?? 0);

                if (activeSubscription != null && LoginMemberSession.UserDetailSession != null)
                {
                    bool isFirstTimeBasic = true;

                    if (planId == (byte)SubscriptionPlanDurationType.IndependentEscortBasic &&
                        LoginMemberSession.UserDetailSession?.SubscriptionPlanDurationType == SubscriptionPlanDurationType.IndependentEscortBasic)
                    {
                        isFirstTimeBasic = false;
                    }

                    LoginSessionModel sessionobj = new LoginSessionModel()
                    {
                        UserId = LoginMemberSession.UserDetailSession?.UserId ?? 0,
                        FirstName = LoginMemberSession.UserDetailSession?.FirstName ?? string.Empty,
                        LastName = LoginMemberSession.UserDetailSession?.LastName ?? string.Empty,
                        DisplayName = LoginMemberSession.UserDetailSession?.DisplayName ?? string.Empty,
                        UserTypeId = LoginMemberSession.UserDetailSession?.UserTypeId ?? 0,
                        EmailId = LoginMemberSession.UserDetailSession?.EmailId ?? string.Empty,
                        SubscriptionPlanPaypalId = activeSubscription.SubscriptionPlanPaypalId,
                        SubscriptionPlanDurationType = (SubscriptionPlanDurationType)activeSubscription.PlanDuration,
                        SubscriptionPlanType = (SubscriptionPlanType)activeSubscription.PlanType,
                        SubscriptionPlanId = activeSubscription.SubscriptionId,
                        SubscriptionPlanExpireDateTime = activeSubscription.ExpiryDateUTC,
                        SubscriptionPlanExpireOn = activeSubscription.ExpiryDateUTC.ToLocal(_httpContextAccessor),
                        AccessToken = LoginMemberSession.UserDetailSession?.AccessToken ?? string.Empty
                    };

                    LoginMemberSession.UserDetailSession = sessionobj;
                    return Json(new { redirect = string.Empty, isFirstTimeBasic, Message = userSubscription?.Message ?? "" });
                }

                return Json(new { redirect = string.Empty });
            }

            string approvalUrl = string.Empty;

            /*
             When user have active subscription then user have to revise plan
             */

            if (LoginMemberSession.UserDetailSession?.SubscriptionPlanExpireDateTime >= System.DateTime.Now && !string.IsNullOrEmpty(LoginMemberSession.UserDetailSession?.SubscriptionPaypalId))
            {
                approvalUrl = await _payPalService.ReviseSubscriptionAsync(userSubscription.Data.SubscriptionPlanPaypalId, returnUrl, cancelUrl, LoginMemberSession.UserDetailSession?.EmailId, LoginMemberSession.UserDetailSession?.UserId.ToString()?.ToEncrypt() ?? "", userSubscription.Data.UserSubscriptionId.ToString().ToEncrypt(), LoginMemberSession.UserDetailSession?.SubscriptionPaypalId ?? "");
            }
            else
            {
                approvalUrl = await _payPalService.CreateSubscriptionAsync(userSubscription.Data.SubscriptionPlanPaypalId, returnUrl, cancelUrl, LoginMemberSession.UserDetailSession?.EmailId, LoginMemberSession.UserDetailSession?.UserId.ToString()?.ToEncrypt() ?? "", userSubscription.Data.UserSubscriptionId.ToString().ToEncrypt());
            }


            if (string.IsNullOrEmpty(approvalUrl))
            {
                return Json(new { redirect = string.Empty });
            }
            return Json(new { redirect = approvalUrl });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSubscription(byte planId, bool isUpgrade = false)
        {
            var cancelStatus = await _subscriptionPlanService.CancelUserSubscriptionByUser(UserId, planId);
            if (cancelStatus.Data)
            {
                if (LoginMemberSession.UserDetailSession != null)
                {
                    var loginMemberSession = LoginMemberSession.UserDetailSession;
                    if (LoginMemberSession.UserDetailSession.UserTypeId == (int)UserTypes.IndependentEscort && !isUpgrade)
                    {
                        await _accountService.SaveFreeSubscription(LoginMemberSession.UserDetailSession.EmailId ?? "");
                        var activeSubscription = await _subscriptionPlanService.GetUserSubscriptionDetail(LoginMemberSession.UserDetailSession?.UserId ?? 0);
                        if (activeSubscription != null)
                        {
                            loginMemberSession.SubscriptionPlanPaypalId = activeSubscription.SubscriptionPlanPaypalId;
                            loginMemberSession.SubscriptionPlanDurationType = (SubscriptionPlanDurationType)activeSubscription.PlanDuration;
                            loginMemberSession.SubscriptionPlanType = (SubscriptionPlanType)activeSubscription.PlanType;
                            loginMemberSession.SubscriptionPlanId = activeSubscription.SubscriptionId;
                            loginMemberSession.SubscriptionPlanExpireDateTime = activeSubscription.ExpiryDateUTC;
                            loginMemberSession.SubscriptionPlanExpireOn = activeSubscription.ExpiryDateUTC.ToLocal(_httpContextAccessor);
                        }
                    }
                    else
                    {

                        loginMemberSession.SubscriptionPlanId = 0;
                        loginMemberSession.SubscriptionPlanDurationType = null;
                        loginMemberSession.SubscriptionPlanType = null;
                        loginMemberSession.SubscriptionPlanExpireDateTime = null;
                        loginMemberSession.SubscriptionPlanPaypalId = string.Empty;
                        loginMemberSession.SubscriptionPaypalId = string.Empty;

                    }
                    LoginMemberSession.UserDetailSession = loginMemberSession;
                }
                return Json(new { status = cancelStatus, message = cancelStatus.Message });
            }
            else
            {
                return Json(new { status = cancelStatus, message = cancelStatus.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateSubscription(byte planId)
        //{
        //    if (LoginMemberSession.UserDetailSession?.SubscriptionPlanId > 0)
        //    {
        //        byte id = Convert.ToByte(LoginMemberSession.UserDetailSession.SubscriptionPlanId);
        //        var cancelStatus = await CancelSubscription(id);

        //        if (cancelStatus.status == true)
        //        {
        //        }
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(byte planId)
        {
            if (LoginMemberSession.UserDetailSession?.SubscriptionPlanId > 0)
            {

                byte id = Convert.ToByte(LoginMemberSession.UserDetailSession.SubscriptionPlanId);
                IActionResult cancelResult = await CancelSubscription(id,true);


                if (cancelResult is JsonResult jsonResult)
                {
                    var resultData = jsonResult.Value as dynamic;

                    if (resultData?.status.Data == true)
                    {
                        var createResult = await CreateSubscription(planId);
                        return createResult;
                    }
                    else
                    {
                        // Handle cancelation failure
                        return Json(new { status = false, message = ResourceString.FailedToCancelSubscription });
                    }
                }
                else
                {
                    // In case the response is not a JsonResult, handle it as an error
                    return Json(new { status = false, message = ResourceString.FailedToCancelSubscription });
                }
            }
            else
            {

                var createResult = await CreateSubscription(planId);
                return createResult;
            }
        }



    }
}
