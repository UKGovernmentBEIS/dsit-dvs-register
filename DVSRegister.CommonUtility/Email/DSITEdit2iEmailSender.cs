using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class DSITEdit2iEmailSender
    {
        private readonly GovUkNotifyApi govUkNotifyApi;
        private readonly GovUkNotifyConfiguration config;

        public DSITEdit2iEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config)
        {
            this.govUkNotifyApi = govUkNotifyApi;
            this.config = config.Value;
        }

        public Task<bool> SendCertificateInfoSubmittedToDSIT()
        {
            var template = config.CabSubmittedDSITEmailTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.LoginLink, config.LoginLink }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> SendAgreementToProceedApplicationToDSIT(string companyName, string serviceName)
        {
            var template = config.AgreementToProceedApplicationToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> SendAgreementToPublishToDIP(string companyName, string serviceName, string recipientName, string emailAddress)
        {
            var template = config.AgreementToPublishTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.RecipientName, recipientName }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> SendAgreementToPublishToDSIT(string companyName, string serviceName)
        {
            var template = config.AgreementToPublishToDSITTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> _2iCheckDeclinedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config._2iCheckDeclinedNotificationToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> _2iCheckApprovedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config._2iCheckApprovedNotificationToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }
    }
}
