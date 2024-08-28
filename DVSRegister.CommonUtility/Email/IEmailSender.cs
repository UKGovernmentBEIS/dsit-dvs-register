using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility.Email
{
    public interface IEmailSender
    {
        public Task<bool> SendEmailConfirmation(string emailAddress, string recipientName);
        public Task<bool> SendEmailConfirmationToOfdia(string expirationDate);
        public Task<bool> SendEmailCabSignUpActivation(string emailAddress, string recipientName);
        public Task<bool> SendEmailCabAccountCreated(string emailAddress, string recipientName);
        public Task<bool> SendEmailCabFailedLoginAttempt(string emailAddress, string timestamp);
        public Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName);
        public Task<bool> SendCertificateInfoSubmittedToDSIT(string emailAddress);

    }
}
