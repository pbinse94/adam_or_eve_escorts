using Shared.Model.Request.Profile;

namespace Business.Communication
{
    public interface IEmailFunctions
    {
        Task EmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink);
        Task EmailVerification14(string toEmail, string emailsubject, string name, string emailVerificationLink);
        Task SendResetPasswordEmail(string emailsubject, string resetUrl, string toEmail, string? name);
        Task SendResetPasswordEmailToWebUser(string emailsubject, string resetUrl, string toEmail, string name);
        Task SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query, string phoneNumber);
        Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model);
        Task PlanPurchasedSuccessMail(string toEmail, string emailsubject, string name, string planName, string purchasedDate, string expiryDate);
        Task PlanChangedSuccessMail(string toEmail, string emailsubject, string name, string planName, string purchasedDate, string expiryDate);
        Task EstablishmentEscortRegistrationAndEmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink, string establishmentCompanyName, string password);
        Task StaffEmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink, string password);
        Task LoginAlert(string toEmail, string emailsubject, string name, string logoutDevicesLink, string deviceInfo, string deviceLocation);
        Task PrfoleSubmittedForSubmission(string userEmail, string emailsubject, string name);
        Task EmailVerificationCode(string toEmail, string emailsubject, string name, string code);
        Task WelcomeEmail(string toEmail, string emailsubject, string name, string message);
    }
}
