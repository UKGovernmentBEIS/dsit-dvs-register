using DVSRegister.CommonUtility.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;

namespace DVSRegister.CommonUtility.Email
{
    public class GovUkNotifyApi : IEmailSender
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


        public async Task<bool> SendEmailConfirmation(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.EmailConfirmationTemplate;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientNamePlaceholder, recipientName }                
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendEmailConfirmationToOfdia(string expirationDate)
        {
            var template = govUkNotifyConfig.ApplicationReceivedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ExpirationDate, expirationDate  },
                { template.LoginLink, govUkNotifyConfig.LoginLink }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress =  govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }
        public async Task<bool> SendEmailCabSignUpActivation(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabSignUpActivationTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  },
                { template.CabSignUpLink, govUkNotifyConfig.CabSignUpLink }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress =  emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendEmailCabAccountCreated(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabAccountCreatedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  },
                { template.CabLoginLink, govUkNotifyConfig.CabLoginLink }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress =  emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendEmailCabFailedLoginAttempt(string emailAddress, string timestamp)
        {
            var template = govUkNotifyConfig.CabFailedLoginAttemptTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.Timestamp, timestamp },
                { template.Email , emailAddress}
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress =  govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendEmailCabInformationSubmitted(string emailAddress, string recipientName)
        {
            var template = govUkNotifyConfig.CabInformationSubmittedTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName, recipientName  }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress =  emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendCertificateInfoSubmittedToDSIT()
        {
            var template = govUkNotifyConfig.CabSubmittedDSITEmailTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.LoginLink,  govUkNotifyConfig.LoginLink}
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                TemplateId = template.Id,
                EmailAddress =  govUkNotifyConfig.OfDiaEmailId,
                Personalisation = personalisation

            };
            return await SendEmail(emailModel);
        }


        #region openong the loop
        public async Task<bool> SendAgreementToProceedApplicationToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.AgreementToProceedApplicationToDSIT;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }
        #endregion

        #region closing the loop
        public async Task<bool> SendAgreementToPublishToDIP(string companyName, string serviceName, string recipientName, string emailAddress)
        {
            var template = govUkNotifyConfig.AgreementToPublishTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {

                { template.ServiceName,  serviceName},
                { template.CompanyName,  companyName},
                { template.RecipientName,  recipientName}
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendAgreementToPublishToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.AgreementToPublishToDSITTemplate;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        #endregion

        #region Remove
        public async Task<bool> SendRemovalRequestConfirmedToDIP(string recipientName, string emailAddress)
        {
            var template = govUkNotifyConfig.ProviderRemovalRequestConfirmed;

            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName,  recipientName}
              
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendProviderRemovalConfirmationToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.ProviderRemovalConfirmationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> SendRecordRemovedToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig.RecordRemovedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {               
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> RemovalRequestDeclinedToProvider(string recipientName, string emailAddress)
        {
            var template = govUkNotifyConfig.RemovalRequestDeclinedToProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientName,  recipientName}
               
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> RemovalRequestDeclinedToDSIT(string companyName, string serviceName)
        {
            var template = govUkNotifyConfig.RemovalRequestDeclinedToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName}
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> _2iCheckDeclinedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {
            var template = govUkNotifyConfig._2iCheckDeclinedNotificationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> _2iCheckApprovedNotificationToDSIT(string companyName, string serviceName, string reasonForRemoval)
        {

            var template = govUkNotifyConfig._2iCheckApprovedNotificationToDSIT;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = govUkNotifyConfig.OfDiaEmailId,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }

        public async Task<bool> RecordRemovedConfirmedToCabOrProvider(string recipientName, string emailAddress, string companyName, string serviceName, string reasonForRemoval)
        {

            var template = govUkNotifyConfig.RecordRemovedConfirmedToCabOrProvider;
            var personalisation = new Dictionary<string, dynamic>
            {
                 { template.RecipientName,  recipientName},
                { template.CompanyName,  companyName},
                { template.ServiceName,  serviceName},
                { template.ReasonForRemoval,  reasonForRemoval},
             };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress ,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            return await SendEmail(emailModel);
        }
        #endregion
    }
}
