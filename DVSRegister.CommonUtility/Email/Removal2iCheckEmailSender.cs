using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class Removal2iCheckEmailSender : EmailSender
    {
        public Removal2iCheckEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        #region Remove
        public async Task<bool> SendRemovalRequestConfirmedToDIP(string recipientName, string emailAddress)
        {
            var template = govUkNotifyConfig.ProviderRemovalRequestConfirmed;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName,  recipientName}

             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> SendProviderRemovalConfirmationToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.ProviderRemovalConfirmationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> SendRecordRemovedToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.RecordRemovedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> RemovalRequestDeclinedToProvider(string recipientName, string emailAddress)
        {
            var template = govUkNotifyConfig.RemovalRequestDeclinedToProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName,  recipientName}

             };
            
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> RemovalRequestDeclinedToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.RemovalRequestDeclinedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> _2iCheckDeclinedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig._2iCheckDeclinedNotificationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> _2iCheckApprovedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {

            var template = govUkNotifyConfig._2iCheckApprovedNotificationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> RecordRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {

            var template = govUkNotifyConfig.RecordRemovedConfirmedToCabOrProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                 { template.RecipientName,  recipientName},
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> RemoveServiceConfirmationToProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.RemoveServiceConfirmationToProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                 { template.RecipientName,  recipientName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> ServiceRemovalConfirmationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.ServiceRemovalConfirmationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                 { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> ServiceRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.ServiceRemovedConfirmedToCabOrProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                 { template.RecipientName,  recipientName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> ServiceRemovedToDSIT(string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.ServiceRemovedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {

                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }
        public async Task<bool> Service2iCheckDeclinedToDSIT(string serviceName)
        {
            var template = govUkNotifyConfig.Service2iCheckDeclinedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
              { template.ServiceName,  serviceName}

             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> Service2iCheckApprovedToDSIT(string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.Service2iCheckApprovedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
              { template.ServiceName,  serviceName},
              { template.ReasonForRemoval,  reasonForRemoval}

             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }


        #endregion
    }
}
