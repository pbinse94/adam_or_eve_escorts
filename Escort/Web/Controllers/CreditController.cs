using Business.Communication;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.Entities;
using Shared.Model.Response;
using Shared.Resources;
using Stripe;
using System.Text;
using System.Text.Json;
using Web.Controllers.Base;
using static Google.Apis.Requests.BatchRequest;

namespace Web.Controllers
{
    //[Route("subscription")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class CreditController : UserBaseController
    {
        private readonly ICreditService _creditService;
        private readonly IProfileService _profileService;

        private readonly PayPalService _payPalService;

        public CreditController(ICreditService creditService, PayPalService payPalService, IProfileService profileService)
        {
            _creditService = creditService;
            _payPalService = payPalService;
            _profileService = profileService;
        }


        public async Task<IActionResult> Index()
        {
            var rangeWiseCreditAmountList = await _creditService.GetCreditPlan();
            int creditBalance = 0;
            if (LoginMemberSession.UserDetailSession != null && LoginMemberSession.UserDetailSession.UserTypeId == (short)UserTypes.Client)
            {
                var userDetail = await _profileService.GetUserDetails(LoginMemberSession.UserDetailSession.UserId);
                creditBalance = userDetail.Data?.CreditBalance ?? 0;
            }
            return View(new Tuple<List<CreditPlan>,int>(new List<CreditPlan>(rangeWiseCreditAmountList), creditBalance));
        }


        [HttpGet]
        public async Task<IActionResult> CalculateCreditPrice(int creditQuantity)
        {
            var totalCalculatedAmount = await _creditService.CalculateCreditPrice(creditQuantity);
            return Json(totalCalculatedAmount);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(int creditQuantity)
        {
            try
            {
                var amount = await _creditService.CalculateCreditPrice(creditQuantity);
                var result = await _payPalService.CreateOrder(amount.Data);

                await _creditService.SaveUserCredit(creditQuantity, result.Id, UserId, Convert.ToDecimal(amount.Data));

                return Json(new { redirect = result.ApproveLink });
            }
            catch (Exception)
            {
                return Json(new { redirect = string.Empty });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Cancel(string token = "")
        {
            _ = await _creditService.UpdateCreditPaymentStatus(token, "failed", UserId);
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Success(string token = "", string PayerID = "")
        {
            var result = await _payPalService.CaptureOrder(token);

            if (result != null && !string.IsNullOrEmpty(result.Id))
            {
                _ = await _creditService.UpdateCreditPaymentStatus(result.Id, result.Status, UserId);
            }

            ViewBag.PaymentSuccess = false;
            if (result != null && !string.IsNullOrEmpty(result.Status) && result.Status.Equals("completed", StringComparison.CurrentCultureIgnoreCase))
            {
                ViewBag.Message = ResourceString.CreditPaymentSuccess;
                ViewBag.PaymentSuccess = true;
            }
            else
            {
                ViewBag.Message = ResourceString.CreditPaymentFailed;
            }
            return View();
        }
    }
}