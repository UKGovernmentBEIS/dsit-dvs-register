using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;
 
namespace DVSRegister.CommonUtility.Email
{
    public class CabTransferEmailSender : EmailSender
    {
        public CabTransferEmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config) : base(govUkNotifyApi, config)
        {
        }
 
        public Task<bool> SendCabTransferConfirmationToCabB(string email, string acceptingCabName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabTransferConfirmationToCabB;          
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.AcceptingCabName, acceptingCabName },
                { template.ProviderName,  providerName },
                { template.ServiceName, serviceName }
            };
            return SendNotification(email, template, personalisation);
        }
        public Task<bool> SendCabTransferConfirmationToCabA(string email, string currentCabName, string acceptingCabName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabTransferConfirmationToCabA;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CurrentCabName, currentCabName},
                { template.AcceptingCabName, acceptingCabName},
                { template.ProviderName,  providerName},
                { template.ServiceName, serviceName }
            };
            return SendNotification(email, template, personalisation);
        }
 
        public Task<bool> SendCabTransferCancellationToCabB(string email, string acceptingCabName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabTransferCancellationToCabB;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.AcceptingCabName, acceptingCabName },
                { template.ProviderName, providerName },
                { template.ServiceName, serviceName}
            };
            return SendNotification(email, template, personalisation);
        }


        public Task<bool> SendCabTransferConfirmationToDSIT(string existingCabName, string acceptingCabName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabTransferConfirmationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ExistingCabName, existingCabName},
                { template.AcceptingCabName, acceptingCabName},
                { template.ProviderName,  providerName},
                { template.ServiceName, serviceName }
            };
            return SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }

        public Task<bool> SendCabTransferCancellationToDSIT(string existingCabName, string decliningCabName, string providerName, string serviceName)
        {
            var template = govUkNotifyConfig.CabTransferCancellationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.DecliningCabName, decliningCabName },
                { template.ExistingCabName, existingCabName },
                { template.ProviderName, providerName },
                { template.ServiceName, serviceName}
            };
            return SendNotificationToOfDiaCommonMailBox(template, personalisation);
        }
    }
}