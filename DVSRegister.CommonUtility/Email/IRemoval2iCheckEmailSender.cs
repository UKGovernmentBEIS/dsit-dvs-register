namespace DVSRegister.CommonUtility.Email
{
    public interface IRemoval2iCheckEmailSender
    {
        Task<bool> SendRemovalRequestConfirmedToDIP(string recipientName, string emailAddress);
        Task<bool> SendProviderRemovalConfirmationToDSIT(string companyName, string serviceName);
        Task<bool> SendRecordRemovedToDSIT(string companyName, string serviceName, string reasonForRemoval);
        Task<bool> RemovalRequestDeclinedToProvider(string recipientName, string emailAddress);
        Task<bool> RemovalRequestDeclinedToDSIT(string companyName, string serviceName);
        Task<bool> RecordRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval);
        Task<bool> RemoveServiceConfirmationToProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval);
        Task<bool> ServiceRemovalConfirmationToDSIT(string companyName, string serviceName, string reasonForRemoval);
    }
}
