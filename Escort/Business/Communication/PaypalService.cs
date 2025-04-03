using Google.Apis.Http;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Model.Response;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.Net.Http;
using Amazon.Runtime;
using Azure;
using Google;

namespace Business.Communication
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _apiBaseUrl;

        public PayPalService(HttpClient httpClient)
        {
            _clientId = SiteKeys.PaypalClientId;
            _clientSecret = SiteKeys.PaypalClientSecret;
            _apiBaseUrl = SiteKeys.BaseURL;
            _httpClient = httpClient;
        }

        // Get PayPal Access Token
        private async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiBaseUrl}/v1/oauth2/token");
            var credentials = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            return jsonResponse.GetProperty("access_token").GetString() ?? string.Empty;
        }

        public async Task<string> CreateSubscriptionAsync(string planId, string returnUrl, string cancelUrl, string? customerEmail, string payerId, string customId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            string modifiedReturnUrl = $"{returnUrl}";

            var requestData = new
            {
                plan_id = planId,
                subscriber = new
                {
                    email_address = customerEmail,
                    payer_id = payerId, // That is UserId
                },
                application_context = new
                {
                    return_url = modifiedReturnUrl,
                    cancel_url = cancelUrl
                },
                custom_id = customId // that is UserSubscriptionId
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/v1/billing/subscriptions", requestData);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            var approvalLink = jsonResponse.GetProperty("links")
                .EnumerateArray()
                .FirstOrDefault(link => link.GetProperty("rel").GetString() == "approve").GetProperty("href").GetString();

            return approvalLink ?? string.Empty;
        }

        public async Task<string> ReviseSubscriptionAsync(string planId, string returnUrl, string cancelUrl, string? customerEmail, string payerId, string customId, string subscriptionPaypalId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            string modifiedReturnUrl = $"{returnUrl}";

            var requestData = new
            {
                plan_id = planId,
                subscriber = new
                {
                    email_address = customerEmail,
                    payer_id = payerId, // That is UserId
                },
                application_context = new
                {
                    return_url = modifiedReturnUrl,
                    cancel_url = cancelUrl
                },
                custom_id = customId // that is UserSubscriptionId
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/v1/billing/subscriptions/{subscriptionPaypalId}/revise", requestData);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            var approvalLink = jsonResponse.GetProperty("links")
                .EnumerateArray()
                .FirstOrDefault(link => link.GetProperty("rel").GetString() == "approve").GetProperty("href").GetString();

            return approvalLink ?? string.Empty;
        }

        public async Task<bool> CancelSubscriptionAsync(string paypalPlanId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        
            var requestData = new
            {
                reason = "unknown"
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/v1/billing/subscriptions/{paypalPlanId}/cancel", requestData);
            response.EnsureSuccessStatusCode();

            return true;
        }

        public async Task<SubscriptionDetails> GetSubscriptionDetailsAsync(string subscriptionId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/v1/billing/subscriptions/{subscriptionId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var subscriptionDetails = JsonConvert.DeserializeObject<SubscriptionDetails>(json); // Deserialize using Newtonsoft.Json
            return subscriptionDetails ?? new SubscriptionDetails();
        }


       
        public async Task<PaypalOrderResponse> CreateOrder(decimal amount=0)
        {
            PaypalOrderResponse? paypalOrderResponse = new();
            string returnUrl = $"{SiteKeys.SiteUrl}{Constants.creditPaymentSuccess}";
            string cancelUrl = $"{SiteKeys.SiteUrl}{Constants.creditPaymentCancel}";

            string idValue = string.Empty;
            string? approvalLink = string.Empty;

            var accessToken = await GetAccessTokenAsync();
            var url = $"{_apiBaseUrl}{Constants.paypalOrderCheckOutUrl}";
            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = Constants.Currency,
                            value = amount
                        },                       
                    }
                },
                application_context = new
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl 
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);
            if(response != null && response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                if(jsonResponse.ValueKind != JsonValueKind.Undefined && jsonResponse.ValueKind != JsonValueKind.Null)
                {
                    if (jsonResponse.TryGetProperty("id", out JsonElement idElement))
                    {
                        idValue = jsonResponse.GetProperty("id").ToString();
                    }                    

                    if (jsonResponse.TryGetProperty("links", out JsonElement linksElement) && linksElement.ValueKind == JsonValueKind.Array)
                    {
                        // Find the 'approve' link in the 'links' array
                        approvalLink = linksElement
                            .EnumerateArray()
                            .FirstOrDefault(link => link.TryGetProperty("rel", out JsonElement relElement) && relElement.GetString() == "approve")
                            .GetProperty("href")
                            .GetString();
                    }                       

                    paypalOrderResponse.Id = idValue;
                    paypalOrderResponse.ApproveLink = approvalLink;
                }
                else
                {
                    paypalOrderResponse.Id = idValue;
                    paypalOrderResponse.ApproveLink = approvalLink;
                }
                
            }
            else
            {
                paypalOrderResponse.Id = idValue;
                paypalOrderResponse.ApproveLink = approvalLink;
            }
            return paypalOrderResponse;
        }


        public async Task<PayPalCaptureOrderResponse> CaptureOrder(string orderID)
        {
            PayPalCaptureOrderResponse payPalCaptureOrderResponse = new();

            var accessToken = await GetAccessTokenAsync();
            var url = $"{_apiBaseUrl}{Constants.paypalOrderCheckOutUrl}{orderID}{Constants.paypalOrderCaptureUrl}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);
            if (response != null && response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (jsonResponse.ValueKind != JsonValueKind.Undefined && jsonResponse.ValueKind != JsonValueKind.Null)
                {
                    if (jsonResponse.TryGetProperty("id", out JsonElement idElement) &&
                        jsonResponse.TryGetProperty("status", out JsonElement statusElement))
                    {
                        payPalCaptureOrderResponse.Id = idElement.ToString();
                        payPalCaptureOrderResponse.Status = statusElement.ToString();
                    }
                    else
                    {
                        payPalCaptureOrderResponse.Id = string.Empty;
                        payPalCaptureOrderResponse.Status = string.Empty;
                    }
                }
                else
                {
                    payPalCaptureOrderResponse.Id = string.Empty;
                    payPalCaptureOrderResponse.Status = string.Empty;
                }
            }
            else
            {
                payPalCaptureOrderResponse.Id = string.Empty;
                payPalCaptureOrderResponse.Status = string.Empty;
            }

            return payPalCaptureOrderResponse;
        }

    }
}