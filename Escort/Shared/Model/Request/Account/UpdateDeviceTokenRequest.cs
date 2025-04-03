namespace Shared.Model.Request.Account
{
    public class ManageLoginAccessDetailRequest
    {
        public string? Email { get; set; }
        public string? DeviceToken { get; set; }
        public short DeviceType { get; set; }
        public string? AccessToken { get; set; }
        public string? DeviceInfo { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
