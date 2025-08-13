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
        #endregion

     
    }
}
