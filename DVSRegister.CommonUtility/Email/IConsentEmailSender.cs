namespace DVSRegister.CommonUtility.Email
{
    public interface IConsentEmailSender
    {
        Task<bool> SendAgreementToProceedApplicationToDSIT(string companyName, string serviceName);
        Task<bool> SendConfirmationToProceedApplicationToDIP(string serviceName, string emailAddress);
        Task<bool> SendDeclineToProceedApplicationToDSIT(string companyName, string serviceName);
        Task<bool> SendConfirmationOfDeclineToProceedApplicationToDIP(string serviceName, string emailAddress);
    }
}
