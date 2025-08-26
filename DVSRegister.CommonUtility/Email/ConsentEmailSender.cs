using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public class ConsentEmailSender : EmailSender
    {
        public ConsentEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }

        #region opening the loop
        public async Task<bool> SendAgreementToProceedApplicationToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.AgreementToProceedApplicationToDSIT;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> SendConfirmationToProceedApplicationToDIP(string serviceName, string emailAddress)
        {
            var template = govUkNotifyConfig.ConfirmationToProceedApplicationToDIP;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ServiceName,  serviceName}
             };
            return await SendNotification(emailAddress, template, personalisation);
        }

        public async Task<bool> SendDeclineToProceedApplicationToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.DeclinedToProceedApplicationToDSIT;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
            };
            return await SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public async Task<bool> SendConfirmationOfDeclineToProceedApplicationToDIP(string serviceName, string emailAddress)
        {
            var template = govUkNotifyConfig.DeclinedToProceedConfirmationToDIP;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ServiceName,  serviceName}
            };
            return await SendNotification(emailAddress, template, personalisation);
        }

        #endregion


    }
}
