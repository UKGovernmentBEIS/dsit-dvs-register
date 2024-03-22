using DVSRegister.CommonUtility.Models;

namespace DVSRegister.CommonUtility.Email
{
    public interface IEmailSender
    {
        public Task<bool> SendEmail(string emailAddress);

    }
}
