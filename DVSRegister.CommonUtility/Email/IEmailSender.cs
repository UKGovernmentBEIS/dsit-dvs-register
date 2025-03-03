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
        public Task<bool> SendCertificateInfoSubmittedToDSIT();

        //opening the loop
        public Task<bool> SendAgreementToProceedApplicationToDSIT(string companyName, string serviceName);

        //closing the loop
        public Task<bool> SendAgreementToPublishToDSIT(string companyName, string serviceName);
        public Task<bool> SendAgreementToPublishToDIP(string companyName, string serviceName, string recipientName, string emailAddress);

        //remove emails

        public Task<bool> SendRemovalRequestConfirmedToDIP(string recipientName, string emailAddress);
        public Task<bool> SendProviderRemovalConfirmationToDSIT(string companyName, string serviceName);
        public Task<bool> SendRecordRemovedToDSIT(string companyName, string serviceName, string reasonForRemoval);

        public Task<bool> RemovalRequestDeclinedToProvider(string recipientName, string emailAddress);
        public Task<bool> RemovalRequestDeclinedToDSIT(string companyName, string serviceName);

        public Task<bool> _2iCheckDeclinedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval);

        public Task<bool> _2iCheckApprovedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval);

        public Task<bool> RecordRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval);

        public Task<bool> RemoveServiceConfirmationToProvider(string recipientName, string emailAddress,string serviceName, string reasonForRemoval);
        public Task<bool > ServiceRemovalConfirmationToDSIT(string companyName, string serviceName, string reasonForRemoval);
        public Task <bool > ServiceRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval);
        public Task<bool> ServiceRemovedToDSIT(string serviceName, string reasonForRemoval);

        public Task<bool> CabServiceRemovalRequested(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval);
        public Task<bool> CabServiceRemovalRequestedToDSIT(string companyName, string serviceName, string reasonForRemoval);
        public Task<bool> RecordRemovalRequestByCabToDSIT(string companyName, string serviceName, string reasonForRemoval);
        public Task<bool> RecordRemovalRequestConfirmationToCab(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval);
        public Task<bool> Service2iCheckDeclinedToDSIT(string serviceName);
        public Task<bool> Service2iCheckApprovedToDSIT(string serviceName, string reasonForRemoval);

        public Task<bool> EditProviderAccepted(string emailAddress, string recipientName, string companyName, string currentData, string previousData);
        public Task<bool> EditProviderDeclined(string emailAddress, string recipientName, string companyName);





    }
}
