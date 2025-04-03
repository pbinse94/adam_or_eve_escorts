using Business.Communication;
using Business.IServices;
using Google.Api.Gax;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shared.Common;
using Shared.Model.Request.Subscription;
using Shared.Model.Response;
using System.Transactions;

namespace Web.Controllers
{
#nullable disable
    public class WebhookController : Controller
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly PayPalService _payPalService;

        public WebhookController(ISubscriptionPlanService subscriptionPlanService, PayPalService payPalService)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _payPalService = payPalService;
        }

        [HttpPost]
        public async Task<IActionResult> Subscription()
        {
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            var webhookEvent = JObject.Parse(requestBody);
            var eventType = webhookEvent["event_type"]?.ToString();
            //Errlake.Crosscutting.ErrLog.LogInfo($"webhook {eventType}", "subscription", "Webhook", webhookEvent.ToString());
            // Extract event type and subscription ID

            var subscriptionId = webhookEvent["resource"]?["id"]?.ToString();
            short transactionStatus = 0;
            if (Enum.TryParse<TransactionStatus>(webhookEvent["resource"]?["status"]?.ToString(), out var transactionStatus1))
            {
                // Convert enum to short
                transactionStatus = (short)transactionStatus1;
            }

            if (eventType == "PAYMENT.SALE.COMPLETED")
            {
                subscriptionId = webhookEvent["resource"]?["billing_agreement_id"]?.ToString();
            }
            var subscriptionDetails = await _payPalService.GetSubscriptionDetailsAsync(subscriptionId);
            var userSubscriptionId = Convert.ToInt32(subscriptionDetails.CustomId.ToDecrypt());


            int result = 0;
            // Handle different event types
            switch (eventType)
            {
                case "BILLING.SUBSCRIPTION.EXPIRED":
                case "BILLING.SUBSCRIPTION.SUSPENDED":
                    // Handle subscription expiration and suspension

                    result = await _subscriptionPlanService.DeactivateUserSubscription(userSubscriptionId, subscriptionId, transactionStatus);
                    break;
                case "BILLING.SUBSCRIPTION.CANCELLED":
                    // Handle subscription cancellation
                    result = await _subscriptionPlanService.CancelUserSubscriptionByWebhook(userSubscriptionId, subscriptionId, 0);
                    break;
                case "BILLING.SUBSCRIPTION.ACTIVATED":
                    result = await _subscriptionPlanService.ActivateUserSubscription(userSubscriptionId, subscriptionId);
                    break;
                case "PAYMENT.SALE.COMPLETED":
                    var savePaymentModel = new SaveSubscriptionPaymentDetailRequest()
                    {
                        UserSubscriptionId = userSubscriptionId,
                        VendorSubscriptionId = subscriptionId,
                        Currency = webhookEvent["resource"]?["amount"]?["currency"]?.ToString(),
                        TransactionAmount = Convert.ToDecimal(webhookEvent["resource"]?["amount"]?["total"]?.ToString()),
                        TransactionFee = Convert.ToDecimal(webhookEvent["resource"]?["transaction_fee"]?["value"]?.ToString()),
                        TransactionId = webhookEvent["resource"]?["id"]?.ToString(),
                        TransactionStatus = webhookEvent["resource"]?["state"]?.ToString(),
                        TransactionDateTimeUTC = string.IsNullOrEmpty(webhookEvent["resource"]?["create_time"]?.ToString()) ? null : Convert.ToDateTime(webhookEvent["resource"]?["create_time"]?.ToString())
                    };
                    result = await _subscriptionPlanService.SaveSubscriptionPaymentDetail(savePaymentModel);
                    break;
                default:
                    // Handle other events
                    break;
            }

            //if (result <= 0)
            //{
            //    Errlake.Crosscutting.ErrLog.LogInfo("webhook DB Error", "subscription", "Webhook info not correct", webhookEvent.ToString());
            //}

            return Ok();
        }


    }
}
