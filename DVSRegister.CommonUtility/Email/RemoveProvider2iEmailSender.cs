using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;


namespace DVSRegister.CommonUtility.Email
{
    public class RemoveProvider2iEmailSender
    {
        private readonly GovUkNotifyApi govUkNotifyApi;
        private readonly GovUkNotifyConfiguration config;

        public RemoveProvider2iEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config)
        {
            this.govUkNotifyApi = govUkNotifyApi;
            this.config = config.Value;
        }

        public Task<bool> SendRemovalRequestConfirmedToDIP(string recipientName)
        {
            var template = config.ProviderRemovalRequestConfirmed;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> SendProviderRemovalConfirmationToDSIT(string companyName, string serviceName)
        {
            var template = config.ProviderRemovalConfirmationToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> SendRecordRemovedToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.RecordRemovedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> RemovalRequestDeclinedToProvider(string recipientName, string emailAddress)
        {
            var template = config.RemovalRequestDeclinedToProvider;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> RemovalRequestDeclinedToDSIT(string companyName, string serviceName)
        {
            var template = config.RemovalRequestDeclinedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> RecordRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.RecordRemovedConfirmedToCabOrProvider;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> RemoveServiceConfirmationToProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval)
        {
            var template = config.RemoveServiceConfirmationToProvider;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> ServiceRemovalConfirmationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.ServiceRemovalConfirmationToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> ServiceRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string serviceName, string reasonForRemoval)
        {
            var template = config.ServiceRemovedConfirmedToCabOrProvider;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> ServiceRemovedToDSIT(string serviceName, string reasonForRemoval)
        {
            var template = config.ServiceRemovedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> CabServiceRemovalRequested(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.CabServiceRemovalRequested;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> CabServiceRemovalRequestedToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.CabServiceRemovalRequestedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> RecordRemovalRequestByCabToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.RecordRemovalRequestByCabToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> RecordRemovalRequestConfirmationToCab(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {
            var template = config.RecordRemovalRequestConfirmationToCab;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.CompanyName, companyName },
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public Task<bool> Service2iCheckDeclinedToDSIT(string serviceName)
        {
            var template = config.Service2iCheckDeclinedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.ServiceName, serviceName }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public Task<bool> Service2iCheckApprovedToDSIT(string serviceName, string reasonForRemoval)
        {
            var template = config.Service2iCheckApprovedToDSIT;
            var personalisation = new Dictionary<string, object>
            {
                { template.ServiceName, serviceName },
                { template.ReasonForRemoval, reasonForRemoval }
            };

            return govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }
    }
}
