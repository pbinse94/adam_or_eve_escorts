using Microsoft.AspNetCore.Http;
using Shared.Common.Enums;
using Shared.Extensions;

namespace Shared.Common
{
    public static class LoginMemberSession
    {
        private static HttpContext HttpContext => new HttpContextAccessor().HttpContext;
        public static LoginSessionModel? UserDetailSession
        {
            get
            {
                var data = HttpContext.Session.GetComplexData<LoginSessionModel>("LoginMemberSession") == null ? null : HttpContext.Session.GetComplexData<LoginSessionModel>("LoginMemberSession");
                return data;
            }
            set
            {
                HttpContext.Session.SetComplexData("LoginMemberSession", value);
            }
        }

    }

    public class LoginSessionModel
    {
        public int UserId { get; set; }
        public string? EmailId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public int UserTypeId { get; set; }
        public string? ProfileImage { get; set; }
        public string? AccessToken { get; set; }
        public short SubscriptionPlanId { get; set; }
        
        public SubscriptionPlanType? SubscriptionPlanType { get; set; }
        public SubscriptionPlanDurationType? SubscriptionPlanDurationType { get; set; }
        public DateTime? SubscriptionPlanExpireDateTime { get; set; }
        public string SubscriptionPlanExpireOn { get; set; } = string.Empty;
        public string SubscriptionPaypalId { get; set; } = string.Empty;
        public string SubscriptionPlanPaypalId { get; set; } = string.Empty;
    }
}
