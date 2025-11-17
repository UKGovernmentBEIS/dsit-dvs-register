using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class CabEmailSender : EmailSender
    {
        public CabEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        public async Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabInformationSubmittedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName },
                { template.ProviderName,  providerName},
                { template.ServiceName,  serviceName},

            };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> SendCertificateInfoSubmittedToDSIT()
        {
            var template = govUkNotifyConfig.CabSubmittedDSITEmailTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.LoginLink,  govUkNotifyConfig.LoginLink}
            };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }


        public async Task<bool> SendEmailContactUpdatesToCab(string emailAddress, string recipientName, string providerName, string previousDate, string newData)
        {
            var template = govUkNotifyConfig.ContactUpdatesToCab;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName },
                { template.ProviderName,  providerName},
                { template.PreviousData,  previousDate},
                { template.CurrentData,  newData}
            };
            return await SendNotification(emailAddress, template, personalisation);
        }
        public async Task<bool> SendEmailContactUpdatesToDSIT(string providerName, string previousDate, string newData)
        {
            var template = govUkNotifyConfig.ContactUpdatesToDSIT;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ProviderName,  providerName},
                { template.PreviousData,  previousDate},
                { template.CurrentData,  newData}
            };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }


    }
}
