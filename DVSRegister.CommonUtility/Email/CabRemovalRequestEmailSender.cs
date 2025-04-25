using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class CabRemovalRequestEmailSender : EmailSender
    {
        public CabRemovalRequestEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }


        public async Task<bool> CabServiceRemovalRequested(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.CabServiceRemovalRequested;
            var personalisation = new Dictionary<string, dynamic>
            {
              { template.RecipientName,  recipientName},
              { template.CompanyName,  companyName},
              { template.ServiceName,  serviceName},
              { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> CabServiceRemovalRequestedToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.CabServiceRemovalRequestedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {

              { template.CompanyName,  companyName},
              { template.ServiceName,  serviceName},
              { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> RecordRemovalRequestByCabToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.RecordRemovalRequestByCabToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
              { template.CompanyName,  companyName},
              { template.ServiceName,  serviceName},
              { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> RecordRemovalRequestConfirmationToCab(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.RecordRemovalRequestConfirmationToCab;
            var personalisation = new Dictionary<string, dynamic>
            {
              { template.RecipientName,  recipientName},
              { template.CompanyName,  companyName},
              { template.ServiceName,  serviceName},
              { template.ReasonForRemoval,  reasonForRemoval},
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }
    }
}
