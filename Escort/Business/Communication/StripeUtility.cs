using Shared.Common;
using Shared.Model.Base;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Communication
{
    public class StripeUtility
    {
        public async Task<ApiResponse<string>> GetCustomerAsync(string customerUniqueId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeSecreatKey;
            
            var service = new CustomerService();
            try
            {
                var response = await service.GetAsync(customerUniqueId);
                if (response != null && response.StripeResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new ApiResponse<string>() { Data = "", Message = "Customer deleted successfully" };
                }
                return new ApiResponse<string>() { Data = "", Message = "Customer deleted successfully" };
               
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>() { Data = "", Message = ex.Message };
            }
        }

        public async Task<ApiResponse<Subscription>> GetSubscriptionAsync(string subscriptionId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeSecreatKey;

            try
            {
                var service = new SubscriptionService();
                var response = await service.GetAsync(subscriptionId);
                if (response != null && response.StripeResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new ApiResponse<Subscription>() { Data = response, Message = "Subscription Data Fetched" };
                }
                return new ApiResponse<Subscription>() { Data = null, Message = "Subscription Data Fetched" };
                

            }
            catch (Exception ex)
            {                
                return new ApiResponse<Subscription>() { Data = null, Message = ex.Message };
            }
        }


        public async Task<ApiResponse<string>> UpdateCustomerDefaultPaymentMethodAsync(string customerUniqueId, string key, string paymentMethodId)
        {
            StripeConfiguration.ApiKey = key;

            // Prepare options for updating the customer
            var options = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            };

            // Initialize a CustomerService instance
            var service = new CustomerService();

            try
            {
                // Attempt to update the customer
                var response = await service.UpdateAsync(customerUniqueId, options);

                if (response != null && response.StripeResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new ApiResponse<string>()
                    {
                        Data = "",
                        Message = "Customer default payment method updated successfully"
                    };
                }
                else
                {
                    // Handle the case where the response is null or the status code is not OK
                    return new ApiResponse<string>()
                    {
                        Data = "",
                        Message = "Failed to update customer default payment method"
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new ApiResponse<string>()
                {
                    Data = "",
                    Message = ex.Message
                };
            }
        }




        public async Task<ApiResponse<(DateTime?, DateTime?)>> GetUpcomingScheduledSubscriptionDatesAsync(string customerId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeSecreatKey;

            try
            {
                var subscriptionScheduleService = new SubscriptionScheduleService();

                // Specify the options for listing subscription schedules
                var options = new SubscriptionScheduleListOptions
                {
                    Customer = customerId,
                    Limit = 1, // Limit to 1 subscription schedule
                    Scheduled = true // Retrieve only upcoming schedules
                };

                // Retrieve the list of subscription schedules
                var response = await subscriptionScheduleService.ListAsync(options);

                if (response.Data.Count > 0)
                {
                    // Retrieve the first (and only) upcoming subscription schedule
                    var upcomingSubscriptionSchedule = response.Data[0];

                    // Check if the subscription schedule has a valid subscription ID
                    if (!string.IsNullOrWhiteSpace(upcomingSubscriptionSchedule.SubscriptionId))
                    {
                        // Retrieve the associated subscription to get start date and end date
                        var subscriptionService = new SubscriptionService();
                        var subscription = await subscriptionService.GetAsync(upcomingSubscriptionSchedule.SubscriptionId);

                        // Extract start date and end date
                        var startDate = subscription.CurrentPeriodStart;
                        var endDate = subscription.CurrentPeriodEnd;

                        // Return success response with start date and end date
                        return new ApiResponse<(DateTime?, DateTime?)>()
                        {
                            Data = (startDate, endDate),
                            Message = "Upcoming Subscription Schedule Dates Fetched"
                        };
                    }
                    else
                    {
                        // Return error response if subscription ID is null or empty
                        return new ApiResponse<(DateTime?, DateTime?)>()
                        {
                            Data = (null, null),
                            Message = "Subscription ID associated with the schedule is invalid"
                        };
                    }
                }
                else
                {
                    // Return success response without data (no upcoming subscription schedules found)
                    return new ApiResponse<(DateTime?, DateTime?)>()
                    {
                        Data = (null, null),
                        Message = "No Upcoming Subscription Schedule Found"
                    };
                }
            }
            catch (Exception ex)
            {
                // Return error response with error message
                return new ApiResponse<(DateTime?, DateTime?)>()
                {
                    Data = (null, null),
                    Message = ex.Message
                };
            }
        }





        public async Task<int> GetCustomerSubscriptionScheduleCountAsync(string customerId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeSecreatKey;

            try
            {
                var options = new SubscriptionScheduleListOptions
                {
                    Customer = customerId,
                    Limit = 100 // Adjust the limit as needed, maximum is 100
                };

                var service = new SubscriptionScheduleService();
                var schedules = await service.ListAsync(options);

                return schedules.Data.Count;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error: {ex.Message}");
                return -1; // Or throw an exception if desired
            }
        }


        public async Task<(DateTime?, DateTime?)> ScheduleSubscriptionAsync(string customerId, string newPlanId)
        {
            StripeConfiguration.ApiKey = SiteKeys.StripeSecreatKey;

            try
            {
                // Retrieve the current subscription
                var subscriptionService = new SubscriptionService();
                var currentSubscriptionOptions = new SubscriptionListOptions
                {
                    Customer = customerId,
                    Status = "all", // Retrieve all subscriptions including canceled ones
                    Limit = 1 // Limit to 1 subscription
                };
                var currentSubscriptions = await subscriptionService.ListAsync(currentSubscriptionOptions);

                if (currentSubscriptions.Data.Count == 0)
                {
                    throw new Exception("No active subscriptions found for the customer.");
                }

                var currentSubscription = currentSubscriptions.Data[0];
                var currentSubscriptionEndDate = currentSubscription.CurrentPeriodEnd;

                // Calculate the start date for the new subscription
                var newSubscriptionStartDate = currentSubscriptionEndDate.AddDays(1);

                // Retrieve the new plan
                var planService = new PlanService();
                var newPlan = await planService.GetAsync(newPlanId);

                // Create a new subscription for the customer
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customerId,
                    Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = newPlanId,
                    },
                },
                    BillingCycleAnchor = currentSubscription.CurrentPeriodEnd, // Start billing cycle immediately

                };
                var newSubscription = await subscriptionService.CreateAsync(subscriptionOptions);

                // Calculate the end date for the new subscription based on the billing cycle of the new plan
                var newSubscriptionEndDate = newSubscription.CurrentPeriodEnd;

                return (newSubscriptionStartDate, newSubscriptionEndDate);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error: {ex.Message}");
                return (null, null);
            }
        }

    }
}
