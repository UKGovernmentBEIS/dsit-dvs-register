using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class Edit2iCheckEmailSender : EmailSender
    {
        public Edit2iCheckEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        #region Edit

        public async Task<bool> EditProviderAccepted(string emailAddress, string recipientName, string companyName, string currentData, string previousData)
        {
            var template = govUkNotifyConfig.EditProviderAccepted;
            var personalisation = new Dictionary<string, dynamic>
            {

                { template.CompanyName,  companyName},
                { template.RecipientName,  recipientName},
                { template.PreviousData,  previousData},
                { template.CurrentData,  currentData},

             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> EditProviderDeclined(string emailAddress, string recipientName, string companyName)
        {
            var template = govUkNotifyConfig.EditProviderDeclined;
            var personalisation = new Dictionary<string, dynamic>
            {

                { template.CompanyName,  companyName},
                { template.RecipientName,  recipientName}

             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> EditServiceAccepted(string emailAddress, string recipientName, string companyName, string serviceName, string currentData, string previousData)
        {
            var template = govUkNotifyConfig.EditServiceAccepted;
            var personalisation = new Dictionary<string, dynamic>
            {

                { template.CompanyName,  companyName},
                 { template.ServiceName,  serviceName},
                { template.RecipientName,  recipientName},
                { template.PreviousData,  previousData},
                { template.CurrentData,  currentData},

             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> EditServiceDeclined(string emailAddress, string recipientName, string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.EditServiceDeclined;
            var personalisation = new Dictionary<string, dynamic>
            {

                { template.CompanyName,  companyName},
                 { template.ServiceName,  serviceName},
                { template.RecipientName,  recipientName}


             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }
        #endregion
    }
}
