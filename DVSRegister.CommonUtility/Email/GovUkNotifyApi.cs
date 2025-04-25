using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;

namespace DVSRegister.CommonUtility.Email
{
    public class GovUkNotifyApi 
    {
        private readonly NotificationClient client;
        private readonly GovUkNotifyConfiguration govUkNotifyConfig;
        private readonly ILogger<GovUkNotifyApi> logger;

        public GovUkNotifyApi(IOptions<GovUkNotifyConfiguration> config, ILogger<GovUkNotifyApi> logger)
        {
            govUkNotifyConfig = config.Value;
            client = new NotificationClient(govUkNotifyConfig.ApiKey);
            this.logger = logger;
        }

        public async Task<bool> CreateModelAndSend(string emailAddress, dynamic template, Dictionary<string, object> personalisation)
        {
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }      



        private async Task<bool> SendEmail(GovUkNotifyEmailModel emailModel)
        {
            try
            {
                await client.SendEmailAsync(
                    emailModel.EmailAddress,
                    emailModel.TemplateId,
                    emailModel.Personalisation,
                    emailModel.Reference,
                    emailModel.EmailReplyToId);
                    return true;
            }
            catch (NotifyClientException e)
            {
                if (e.Message.Contains("Not a valid email address"))
                {
                    logger.LogWarning("GOV.UK Notify could not send to an invalid email address");
                }
                else if (e.Message.Contains("send to this recipient using a team-only API key"))
                {
                    // In development we use a 'team-only' API key which can only send to team emails
                    logger.LogWarning("GOV.UK Notify cannot send to this recipient using a team-only API key");
                }
                else
                {
                    logger.LogError(e, "GOV.UK Notify returned an error");
                }
                return false;
            }
        }


        
      
    }
}
