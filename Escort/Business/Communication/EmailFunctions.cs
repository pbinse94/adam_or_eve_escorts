using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Common;
using Shared.Model.Entities;
using Shared.Model.Request.Profile;

namespace Business.Communication
{
    public class EmailFunctions : IEmailFunctions
    {
        private readonly EmailConfigurationKeys _configurationKey;
        private readonly string _emailTemplatePath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _logoPath;
        private readonly IEmailHelperCore _emailHelper;
        public EmailFunctions(IOptions<EmailConfigurationKeys> configurationKey, IEmailHelperCore emailHelperCore, IHttpContextAccessor httpContextAccessor)
        {
            _configurationKey = configurationKey.Value;
            _emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate");
            _logoPath = string.Format("{0}assets/images/whiteLogo.png", SiteKeys.SiteUrl);
            _emailHelper = emailHelperCore;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// By this method we can send email through deligate
        /// </summary>
        /// <param name="emailHelper"></param>
        public async Task SendEmailThroughDelegate(string body, string recipient, string subject, string recipientCC, string recipientBCC)
        {
            await Task.Factory.StartNew(async () =>
            {
                await _emailHelper.Send(body, recipient, subject, recipientCC, recipientBCC);
            });
        }
        public async Task EmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "Registration.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##EmailVerificationLink##", emailVerificationLink),
                    new MessageKeyValue("##AppName##", Constants.AppName)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }

        public async Task EstablishmentEscortRegistrationAndEmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink, string establishmentCompanyName, string password)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "EstablishmentEscortRegistration.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##EmailVerificationLink##", emailVerificationLink),
                    new MessageKeyValue("##AppName##", Constants.AppName),
                    new MessageKeyValue("##Password##", password),
                    new MessageKeyValue("##Logo##", _logoPath),
                    new MessageKeyValue("##EstablishmentCompanyName##", establishmentCompanyName)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }

        public async Task SendResetPasswordEmail(string emailsubject, string resetUrl, string toEmail, string? name)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "ResetPassword.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                        new MessageKeyValue("##Name##", name),
                        new MessageKeyValue("##ResetUrl##", resetUrl),
                        new MessageKeyValue("##ImagePath##", _logoPath),
                        new MessageKeyValue("##AppName##", Constants.AppName)
                        );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
        public async Task SendResetPasswordEmailToWebUser(string emailsubject, string resetUrl, string toEmail, string name)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "ResetPassword.html");

            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                new MessageKeyValue("##Name##", name),
                new MessageKeyValue("##ResetUrl##", resetUrl),
                new MessageKeyValue("##ImagePath##", _logoPath),
                new MessageKeyValue("##AppName##", Constants.AppName)
                );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
        public async Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "ContactEscort.html");

            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,

          new MessageKeyValue("##Name##", model.Name),
          new MessageKeyValue("##Email##", model.Email),
          new MessageKeyValue("##DisplayName##", model.DisplayName),
          new MessageKeyValue("##PhoneNumber##", model.PhoneNumber),
          new MessageKeyValue("##Day##", model.Day),
          new MessageKeyValue("##Time##", model.Time.ToString()),
          new MessageKeyValue("##Duration##", model.Duration.ToString()),
          new MessageKeyValue("##Service##", model.Service),
          new MessageKeyValue("##MeetLocation##", model.MeetLocation),
          new MessageKeyValue("##IsFirstTime##", model.IsFirstTime ? "New Client" : "Old Client"),
          new MessageKeyValue("##ContactMethod##", model.ContactMethod),
          new MessageKeyValue("##ImagePath##", _logoPath),
          new MessageKeyValue("##AppName##", Constants.AppName)
          );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = model.EscortMail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
        public async Task SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query, string phoneNumber)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "ContactUs.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                            new MessageKeyValue("##UserName##", userName),
                            new MessageKeyValue("##UserEmail##", userEmail),
                            new MessageKeyValue("##Query##", query),
                            new MessageKeyValue("##UserPhoneNumber##", string.IsNullOrEmpty(phoneNumber) ? "" : phoneNumber),
                            new MessageKeyValue("##ImagePath##", _logoPath),
                            new MessageKeyValue("##AppName##", Constants.AppName)
                            );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = _configurationKey.AdminEmail ?? string.Empty;
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }

        public async Task PlanPurchasedSuccessMail(string toEmail, string emailsubject, string name, string planName, string purchasedDate, string expiryDate)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "PlanPurchased.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##PlanName##", planName),
                    new MessageKeyValue("##PurchaseDate##", purchasedDate),
                    new MessageKeyValue("##ExpiryDate##", string.IsNullOrEmpty(expiryDate) ? "" : $"<p><b>Expiry Date</b> : {expiryDate}</p>"),
                    new MessageKeyValue("##Logo##", _logoPath)
                   );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string subject = emailsubject;

            await SendEmailThroughDelegate(body, toEmail, subject, recipientCC, recipientBCC);
        }

        public async Task PlanChangedSuccessMail(string toEmail, string emailsubject, string name, string planName, string purchasedDate, string expiryDate)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "PlanChanged.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                new MessageKeyValue("##Name##", name),
                new MessageKeyValue("##PlanName##", planName),
                new MessageKeyValue("##PurchaseDate##", purchasedDate),
                new MessageKeyValue("##ExpiryDate##", expiryDate),
                new MessageKeyValue("##Logo##", _logoPath)
               );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string subject = emailsubject;

            await SendEmailThroughDelegate(body, toEmail, subject, recipientCC, recipientBCC);
        }

        public async Task EmailVerification14(string toEmail, string emailsubject, string name, string emailVerificationLink)
        {
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate("test", recipient, subject, string.Empty, string.Empty);
        }

        public async Task StaffEmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink, string password)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "StaffEmailVerification.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##EmailVerificationLink##", emailVerificationLink),
                    new MessageKeyValue("##Email##", toEmail),
                    new MessageKeyValue("##Password##", password),
                    new MessageKeyValue("##ImagePath##", _logoPath),
                    new MessageKeyValue("##AppName##", Constants.AppName)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }


        public async Task LoginAlert(string toEmail, string emailsubject, string name, string logoutDevicesLink, string deviceInfo, string deviceLocation)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "LoginAlert.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##LogoutLink##", logoutDevicesLink),
                    new MessageKeyValue("##AppName##", Constants.AppName),
                    new MessageKeyValue("##DeviceInfo##", deviceInfo),
                    new MessageKeyValue("##Logo##", _logoPath),
                    new MessageKeyValue("##LoginTime##", DateTime.UtcNow.ToLocal(_httpContextAccessor)),
                    new MessageKeyValue("##DeviceLocation##", deviceLocation)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }
        public async Task PrfoleSubmittedForSubmission(string userEmail, string emailsubject, string name)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "ProfileSubmission.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                     new MessageKeyValue("##Logo##", _logoPath),
                    new MessageKeyValue("##UserEmail##", userEmail),
                    new MessageKeyValue("##SubmissionDate##", DateTime.UtcNow.ToLocal(_httpContextAccessor)),
                    new MessageKeyValue("##AppName##", Constants.AppName)
                );
            string recipient = _configurationKey?.AdminEmail?.ToLower() ?? string.Empty;
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }

        public async Task EmailVerificationCode(string toEmail, string emailsubject, string name, string code)
        {
            //var serverFilePath = Path.Combine(_emailTemplatePath, "EmailVerificationCode.html");
            //string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
            //        new MessageKeyValue("##Name##", name),
            //        new MessageKeyValue("##Code##", code),
            //        new MessageKeyValue("##AppName##", Constants.AppName),
            //        new MessageKeyValue("##Logo##", _logoPath)
            //    );
            //string recipient = toEmail.ToLower();
            //string subject = emailsubject;
            //await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);

            //var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmailTemplate");

            var serverFilePath = Path.Combine(_emailTemplatePath, "EmailVerificationCode.html");

            // ✅ Check if email template file exists
            if (!File.Exists(serverFilePath))
            {
                throw new FileNotFoundException($"Email template not found at: {serverFilePath}");
            }

            // ✅ Ensure email is valid
            if (string.IsNullOrEmpty(toEmail) || !toEmail.Contains("@"))
            {
                throw new ArgumentException("Invalid recipient email.");
            }

            // ✅ Generate email content
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                new MessageKeyValue("##Name##", name),
                new MessageKeyValue("##Code##", code),
                new MessageKeyValue("##AppName##", Constants.AppName),
                new MessageKeyValue("##Logo##", _logoPath)
            );

            string recipient = toEmail.ToLower();
            string subject = emailsubject;

            // ✅ Send email asynchronously
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }
        public async Task WelcomeEmail(string toEmail, string emailsubject, string name,string message)
        {
            var serverFilePath = Path.Combine(_emailTemplatePath, "WelcomeEmail.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name), 
                    new MessageKeyValue("##AppName##", Constants.AppName),
                    new MessageKeyValue("##Message##", message),
                    new MessageKeyValue("##Logo##", _logoPath)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            await SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }
    }


}
