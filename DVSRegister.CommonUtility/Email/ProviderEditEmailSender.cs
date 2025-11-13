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
    }
}
