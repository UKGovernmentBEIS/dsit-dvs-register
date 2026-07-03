namespace DVSRegister.CommonUtility.Email
{
    public interface ICabEmailSender
    {
        Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName, string providerName,
            string serviceName);

        Task<bool> SendCertificateInfoSubmittedToDSIT();
    }
}