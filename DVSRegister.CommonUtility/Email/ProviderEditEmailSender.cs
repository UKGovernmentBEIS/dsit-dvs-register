using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class ProviderEditEmailSender : EmailSender
    {
        public ProviderEditEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        public Task<bool> SendProviderEditRequestSubmittedToCab(string email, string providerName, string previousData, string currentData)
        {
            var template = govUkNotifyConfig.ProviderEditRequestSubmitted;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, email },
                { template.ProviderName, providerName },
                { template.PreviousData, previousData },
                { template.CurrentData, currentData}
            };
            return SendNotification(email, template, personalisation);
        }
        public Task<bool> SendProviderEditRequestSubmittedToOfdia(string providerName, string previousData, string currentData)
        {
            var template = govUkNotifyConfig.ProviderEditRequestReceived;
            var personalisation = new Dictionary<string, dynamic>
            {            
                { template.ProviderName, providerName },
                { template.PreviousData, previousData },
                { template.CurrentData, currentData}
            };
            return SendNotificationToOfDiaCommonMailBox(template, personalisation);
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
