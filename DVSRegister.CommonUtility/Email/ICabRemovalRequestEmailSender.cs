namespace DVSRegister.CommonUtility.Email
{
    public interface ICabRemovalRequestEmailSender
    {
        Task<bool> CabServiceRemovalRequested(string recipientName, string emailAddress, string companyName,
            string serviceName, string reasonForRemoval);

        Task<bool> CabServiceRemovalRequestedToDSIT(string companyName, string serviceName, string reasonForRemoval);
        Task<bool> RecordRemovalRequestByCabToDSIT(string companyName, string serviceName, string reasonForRemoval);

        Task<bool> RecordRemovalRequestConfirmationToCab(string recipientName, string emailAddress, string companyName,
            string serviceName, string reasonForRemoval);

        Task<bool> RemovalRequestCancelledToCab(string emailAddress, string cabName, string companyName,
            string serviceName);

        Task<bool> RemovalRequestCancelledToDSIT(string cabName, string providerName, string serviceName);
    }
}