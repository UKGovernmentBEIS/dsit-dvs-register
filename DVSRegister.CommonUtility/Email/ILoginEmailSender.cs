namespace DVSRegister.CommonUtility.Email
{
    public interface ILoginEmailSender
    {
        Task<bool> SendEmailCabAccountCreated(string emailAddress, string recipientName);
        Task<bool> SendEmailCabAccountCreatedToDSIT(string recipientName, string recipientEmail, string cabEmail, string? cabName);
        Task<bool> SendEmailCabFailedLoginAttempt(string emailAddress, string timestamp);
    }
}
