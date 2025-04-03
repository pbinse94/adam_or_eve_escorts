using Newtonsoft.Json;

namespace Shared.Model.Response
{
# nullable disable
    public class SubscriptionDetails
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_update_time")]
        public DateTime StatusUpdateTime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }

        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("shipping_amount")]
        public ShippingAmount ShippingAmount { get; set; }

        [JsonProperty("subscriber")]
        public Subscriber Subscriber { get; set; }

        [JsonProperty("billing_info")]
        public BillingInfo BillingInfo { get; set; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("plan_overridden")]
        public bool PlanOverridden { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public class ShippingAmount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Subscriber
    {
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }

    public class Name
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    public class ShippingAddress
    {
        [JsonProperty("address")]
        public Address Address { get; set; }
    }

    public class Address
    {
        [JsonProperty("address_line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("address_line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("admin_area_2")]
        public string AdminArea2 { get; set; }

        [JsonProperty("admin_area_1")]
        public string AdminArea1 { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }

    public class BillingInfo
    {
        [JsonProperty("outstanding_balance")]
        public OutstandingBalance OutstandingBalance { get; set; }

        [JsonProperty("cycle_executions")]
        public List<CycleExecution> CycleExecutions { get; set; }

        [JsonProperty("last_payment")]
        public LastPayment LastPayment { get; set; }

        [JsonProperty("next_billing_time")]
        public DateTime NextBillingTime { get; set; }

        [JsonProperty("failed_payments_count")]
        public int FailedPaymentsCount { get; set; }
    }

    public class OutstandingBalance
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class CycleExecution
    {
        [JsonProperty("tenure_type")]
        public string TenureType { get; set; }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("cycles_completed")]
        public int CyclesCompleted { get; set; }

        [JsonProperty("cycles_remaining")]
        public int CyclesRemaining { get; set; }

        [JsonProperty("current_pricing_scheme_version")]
        public int CurrentPricingSchemeVersion { get; set; }

        [JsonProperty("total_cycles")]
        public int TotalCycles { get; set; }
    }

    public class LastPayment
    {
        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }

    public class Amount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }
}
