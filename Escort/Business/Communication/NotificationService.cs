using Shared.Common;
using Shared.Model.Request.Profile;
using Shared.Resources;

namespace Business.Communication
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailFunctions _emailFunctions;

        public NotificationService(IEmailFunctions emailFunctions)
        {
            _emailFunctions = emailFunctions;
        }

        public async Task EmailVerification(string toEmail, string name, string token, string subject, int devicetype)
        {
            var emailConfirmationLink = $"{SiteKeys.SiteUrl}Account/EmailVerification?token={System.Web.HttpUtility.UrlEncode(token)}&type={devicetype}";
            await _emailFunctions.EmailVerification(toEmail, subject, name, emailConfirmationLink);
        }
        public async Task EstablishmentEscortRegistrationAndEmailVerification(string toEmail, string name, string token, string subject, int devicetype, string establishmentCompanyName, string password)
        {
            var emailConfirmationLink = $"{SiteKeys.SiteUrl}Account/EmailVerification?token={System.Web.HttpUtility.UrlEncode(token)}&type={devicetype}";
            await _emailFunctions.EstablishmentEscortRegistrationAndEmailVerification(toEmail, subject, name, emailConfirmationLink, establishmentCompanyName, password);
        }

        public async Task SendResetPasswordEmail(string emailsubject, string token, string toEmail, string? name)
        {
            var reseturl = $"{SiteKeys.SiteUrl}Account/ResetPassword?token={token}";
            await _emailFunctions.SendResetPasswordEmail(ResourceString.ForgetPasswordSubject, reseturl, toEmail, name);
        }

        public async Task SendResetPasswordEmailToWebUser(string emailsubject, string token, string toEmail, string name)
        {
            var passwordResetLink = SiteKeys.SiteUrl + "Account/ResetPassword?Token=" + token;
            await _emailFunctions.SendResetPasswordEmailToWebUser(ResourceString.ForgetPasswordSubject, passwordResetLink, toEmail, name);
        }

        public async Task SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query, string phoneNumber)
        {
            await _emailFunctions.SendContactUsMailToAdmin(emailsubject, userName, userEmail, query, phoneNumber);
        }
        public async Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model)
        {
            await _emailFunctions.SendContactUsMailToEscort(emailsubject, model);
        }
        // admin users

        public async Task StaffEmailVerification(string toEmail, string name, string token, string subject, string password)
        {
            var emailConfirmationLink = $"{SiteKeys.SiteUrl}Account/EmailVerification?token={System.Web.HttpUtility.UrlEncode(token)}";
            await _emailFunctions.StaffEmailVerification(toEmail, subject, name, emailConfirmationLink, password);
        }

        public async Task LoginInAnotherDevice(string toEmail, string name, string token, string subject, string deviceInfo, string deviceLocation)
        {
            var logoutAllDeviceLink = $"{SiteKeys.SiteUrl}Account/LogoutFromAllDevices?accesstoken={System.Web.HttpUtility.UrlEncode(token)}";
            await _emailFunctions.LoginAlert(toEmail, subject, name, logoutAllDeviceLink, deviceInfo, deviceLocation);
        }

        public async Task ProfileSubmittedForApproval(string name, string useremail, string subject)
        {
            await _emailFunctions.PrfoleSubmittedForSubmission(useremail, subject, name);
        }
        public async Task EmailVerificationCode(string toEmail, string name, string code, string subject, int devicetype)
        {
            
            await _emailFunctions.EmailVerificationCode(toEmail, subject, name, code);
        }
        public async Task WelcomeEmail(string toEmail, string name,string subject,string message)
        { 
            await _emailFunctions.WelcomeEmail(toEmail, subject, name,message);
        }
    }
}
