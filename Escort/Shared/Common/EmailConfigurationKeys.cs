namespace Shared.Common
{
    public class EmailConfigurationKeys
    {
        public string? MailServer { get; set; }
        public int Port { get; set; }
        public string? MailAuthUser { get; set; }
        public string? MailAuthPass { get; set; }
        public bool EnableSSL { get; set; }
        public string? EmailFromAddress { get; set; }
        public string? EmailFromName { get; set; }
        public string? EmailBCC { get; set; }
        public string? EmailCC { get; set; }
        public string? AdminEmail { get; set; }
    }
}
