using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class LoginEmailSender
    {
        private readonly GovUkNotifyApi govUkNotifyApi;
        private readonly GovUkNotifyConfiguration config;

        public LoginEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config)
        {
            this.govUkNotifyApi = govUkNotifyApi;
            this.config = config.Value;
        }

        public async Task<bool> SendEmailConfirmation(string emailAddress, string recipientName)
        {
            var template = config.EmailConfirmationTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientNamePlaceholder, recipientName }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> SendEmailConfirmationToOfdia(string expirationDate)
        {
            var template = config.ApplicationReceivedTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.ExpirationDate, expirationDate },
                { template.LoginLink, config.LoginLink }
            };

            return await govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public async Task<bool> SendEmailCabSignUpActivation(string emailAddress, string recipientName)
        {
            var template = config.CabSignUpActivationTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.CabSignUpLink, config.CabSignUpLink }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> SendEmailCabAccountCreated(string emailAddress, string recipientName)
        {
            var template = config.CabAccountCreatedTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName },
                { template.CabLoginLink, config.CabLoginLink }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> SendEmailCabFailedLoginAttempt(string emailAddress, string timestamp)
        {
            var template = config.CabFailedLoginAttemptTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.Timestamp, timestamp },
                { template.Email, emailAddress }
            };

            return await govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }

        public async Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName)
        {
            var template = config.CabInformationSubmittedTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.RecipientName, recipientName }
            };

            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

        public async Task<bool> SendCertificateInfoSubmittedToDSIT()
        {
            var template = config.CabSubmittedDSITEmailTemplate;
            var personalisation = new Dictionary<string, object>
            {
                { template.LoginLink, config.LoginLink }
            };

            return await govUkNotifyApi.CreateModelAndSend(config.OfDiaEmailId, template, personalisation);
        }
    }
}
