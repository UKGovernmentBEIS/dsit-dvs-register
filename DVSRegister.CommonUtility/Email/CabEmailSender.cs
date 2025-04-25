using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class CabEmailSender : EmailSender
    {
        public CabEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        public async Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabInformationSubmittedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  }
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
    }
}
