using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Common
{
#nullable disable
    public class SiteKeys
    {
        public static int UtcOffsetInSecond { get; set; }
        public static string DeviceToken { get; set; }
        public static string SitePhysicalPath { get; set; }
        public static string SiteUrl { get; set; }
        public static string FrontSiteUrl { get; set; }
        public static int UtcOffset { get; set; } = 0;
        public static int UtcOffsetInSecond_API { get; set; }
        public static string AccessToken { get; set; }
        public static string FCMServerKey { get; set; }
        public static string FCMSenderId { get; set; }
        public static string EncryptDecryptKey { get; set; }
        public static string GeoLocationApiKey { get; set; }

        public static string StripeSecreatKey { get; set; }
        public static int RememberMeCookieTimeMinutes { get; set; }

        public static string PaypalClientId { get; set; }
        public static string PaypalClientSecret { get; set; }
        public static string BaseURL { get; set; }
        public static decimal AdminPercentage { get; set; }
        public static bool IsLive { get; set; }
        public static int HttpsPort { get; set; }
    }

    public static class Constants
    {
        public const int DefultPageNumber = 1;
        public const int DefultPageSize = 10;
        public const string DefaultUserImage = "assets/images/Defaultimage.png";
        public const string UserImageFolderPath = "Uploads/UserImages/";
        public const string UserVideoFolderPath = "Uploads/UserVideos/";
        public const string DefaultUserPng = "DefaultImage.png";
        public const string EmailTempaltePath = "wwwroot\\EmailTemplate";
        public const string AppName = "Adam or Eve Escorts";        
        public const string ProfileImage = "user/profile/";
        public const string UserImages = "user/detail/";
        public const string UserVideos = "user/";
        public const string CurrencySymbol = "$";
        public const string Currency = "USD";
        public const string creditPaymentSuccess = "Credit/Success";
        public const string creditPaymentCancel = "Credit/Cancel";
        public const string paypalOrderCheckOutUrl = "/v2/checkout/orders/";
        public const string paypalOrderCaptureUrl = "/capture";

        public const string Clients = "Clients";
        public const string Escorts = "Escorts";
        public const string Establishments = "Establishments";
        public const string RevenueReport = "Revenue Report";
        public const string Dashboard = "Dashboard";
    }
}
