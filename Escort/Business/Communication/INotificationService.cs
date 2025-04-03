using Shared.Model.Request.Profile;

namespace Business.Communication
{
    public interface INotificationService
    {
        Task EmailVerification(string toEmail, string name, string token, string subject, int devicetype);
        Task SendResetPasswordEmail(string emailsubject, string token, string toEmail, string? name);
        Task SendResetPasswordEmailToWebUser(string emailsubject, string token, string toEmail, string name);
        Task SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query, string phoneNumber);
        Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model);
        Task EstablishmentEscortRegistrationAndEmailVerification(string toEmail, string name, string token, string subject, int devicetype, string establishmentCompanyName, string password);
        Task StaffEmailVerification(string toEmail, string name, string token, string subject,string password);
        Task LoginInAnotherDevice(string toEmail, string name, string token, string subject, string deviceInfo, string deviceLocation);
        Task ProfileSubmittedForApproval(string name, string useremail, string subject);
        Task EmailVerificationCode(string toEmail, string name, string code, string subject, int devicetype);
        Task WelcomeEmail(string toEmail, string name, string subject, string message);
    }
}
