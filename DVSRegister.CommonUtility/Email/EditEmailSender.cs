using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class EditEmailSender
    {
        private readonly GovUkNotifyApi govUkNotifyApi;
        private readonly GovUkNotifyConfiguration config;

        public EditEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config)
        {
            this.govUkNotifyApi = govUkNotifyApi;
            this.config = config.Value;
        }
        
        public async Task<bool> EditProviderAccepted(string emailAddress, string recipientName, string companyName, string currentData, string previousData)
        {
            var template = config.EditProviderAccepted;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.RecipientName, recipientName },
                { template.PreviousData, previousData },
                { template.CurrentData, currentData }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> EditProviderDeclined(string emailAddress, string recipientName, string companyName)
        {
            var template = config.EditProviderDeclined;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.RecipientName, recipientName }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> EditServiceAccepted(string emailAddress, string recipientName, string companyName, string serviceName, string currentData, string previousData)
        {
            var template = config.EditServiceAccepted;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.RecipientName, recipientName },
                { template.PreviousData, previousData },
                { template.CurrentData, currentData }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> EditServiceDeclined(string emailAddress, string recipientName, string companyName, string serviceName)
        {
            var template = config.EditServiceDeclined;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.RecipientName, recipientName }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }
    }
}
