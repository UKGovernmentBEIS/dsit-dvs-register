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
    }
}
