using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class LoginEmailSender : EmailSender
    {
        public LoginEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        public async Task<bool> SendEmailCabSignUpActivation(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabSignUpActivationTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  },
                { template.CabSignUpLink, govUkNotifyConfig.CabSignUpLink }
            };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> SendEmailCabAccountCreated(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabAccountCreatedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  },
                { template.CabLoginLink, govUkNotifyConfig.CabLoginLink }
            };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> SendEmailCabFailedLoginAttempt(string emailAddress, string timestamp)
        {
            var template = govUkNotifyConfig.CabFailedLoginAttemptTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.Timestamp, timestamp },
                { template.Email , emailAddress}
            };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }
    }
}
