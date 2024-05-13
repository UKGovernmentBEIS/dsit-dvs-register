using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility.Email
{
    public interface IEmailSender
    {
        public Task<bool> SendEmailConfirmation(string emailAddress, string recipientName);
        public Task<bool> SendEmailConfirmationToOfdia(string expirationDate);

    }
}
