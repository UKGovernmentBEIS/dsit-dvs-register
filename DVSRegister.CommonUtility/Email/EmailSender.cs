using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Options;

namespace DVSRegister.CommonUtility.Email
{
    public abstract class EmailSender(GovUkNotifyApi govUkNotifyApi, IOptions<GovUkNotifyConfiguration> config)
    {
        protected readonly GovUkNotifyApi govUkNotifyApi = govUkNotifyApi;
        protected readonly GovUkNotifyConfiguration govUkNotifyConfig = config.Value;


        protected async Task<bool> SendNotificationToOfDiaCommonMailBox<TTemplate>(TTemplate template, Dictionary<string, dynamic> personalisation)
        {
            return await govUkNotifyApi.CreateModelAndSend(govUkNotifyConfig.OfDiaEmailId, template, personalisation);
        }

        protected async Task<bool> SendNotification<TTemplate>(string emailAddress, TTemplate template, Dictionary<string, dynamic> personalisation)
        {
            return await govUkNotifyApi.CreateModelAndSend(emailAddress, template, personalisation);
        }

       
    }
}
