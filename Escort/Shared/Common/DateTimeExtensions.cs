using Microsoft.AspNetCore.Http;
using Shared.Extensions;

namespace Shared.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime ToUtc(this DateTime dt, IHttpContextAccessor httpContextAccessor) // convert local time into UTC time
        {
            return dt.AddSeconds(GetOffsetSecond_FromKey(httpContextAccessor) * -1);
        }
        public static string ToLocal(this DateTime dt, IHttpContextAccessor httpContextAccessor) // convert UTC into local time
        {
            return dt.AddSeconds(GetOffsetSecond_FromKey(httpContextAccessor)).ToString("dd MMM yyyy hh:mm tt");
        }

        public static string ToLocalDatePart(this DateTime dt, IHttpContextAccessor httpContextAccessor) // convert UTC into local time
        {
            return dt.AddSeconds(GetOffsetSecond_FromKey(httpContextAccessor)).ToString("dd MMM yyyy");
        }


        public static Int32 GetOffsetSecond_FromKey(IHttpContextAccessor httpContextAccessor)
        {
            if (SiteKeys.UtcOffsetInSecond_API != 0)
            {
                return SiteKeys.UtcOffsetInSecond_API;
            }
            else
            {
                return httpContextAccessor.HttpContext.Session.GetInt32("UtcOffsetInSecond") ?? 0;
            }
        }
    }
}
