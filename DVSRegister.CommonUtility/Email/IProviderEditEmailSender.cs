namespace DVSRegister.CommonUtility.Email;

public interface IProviderEditEmailSender
{
    Task<bool> SendProviderEditRequestSubmittedToCab(string email, string providerName, string previousData, string currentData);
    Task<bool> SendProviderEditRequestSubmittedToOfdia(string providerName, string previousData, string currentData);
    Task<bool> SendEmailContactUpdatesToCab(string emailAddress, string recipientName, string providerName, string previousDate, string newData);
    Task<bool> SendEmailContactUpdatesToDSIT(string providerName, string previousDate, string newData);
}