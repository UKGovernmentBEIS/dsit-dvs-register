using AutoMapper;
using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class RemoveProvider2iService :IRemoveProvider2iService 
    {

        private readonly IRemoveProvider2iRepository removeProvider2iRepository;
        private readonly IRemoveProviderService removeProviderService;
        private readonly IMapper mapper;
        private readonly Removal2iCheckEmailSender emailSender;
        private readonly ICabRepository cabRepository;


        public RemoveProvider2iService(IRemoveProvider2iRepository removeProvider2iRepository, ICabRepository cabRepository, IMapper mapper, Removal2iCheckEmailSender emailSender, IRemoveProviderService removeProviderService)
        {
            this.removeProviderService = removeProviderService;
            this.removeProvider2iRepository = removeProvider2iRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.cabRepository = cabRepository;
        }
        public async Task<ProviderProfileDto?> GetProviderAndServiceDetailsByRemovalToken(string token, string tokenId)
        {
            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (removeProviderToken.Provider != null)
            {
                var provider = await removeProvider2iRepository.GetProviderDetails(removeProviderToken.ProviderProfileId);
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count>0)
                {
                    var mappedServiceIds = removeProviderToken.RemoveTokenServiceMapping.Where(mapping => mapping.RemoveProviderTokenId == removeProviderToken.Id).Select(mapping => mapping.ServiceId).ToList();
                    ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
                    providerProfileDto.Services = providerProfileDto.Services.Where(service => mappedServiceIds.Contains(service.Id) && service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation).ToList();
                    return providerProfileDto;

                }
                else
                {
                    ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);
                    providerProfileDto.Services = providerProfileDto.Services.Where(service => service.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation).ToList();
                    return providerProfileDto;
                }
             
            }
            else
            {
                return null;
            }
        }



        public async Task<TokenStatusEnum> GetTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum tokenStatus = TokenStatusEnum.NA;

            var provider = await removeProvider2iRepository.GetProviderDetails(tokenDetails.ProviderProfileId);

            if(tokenDetails.ServiceIds!=null)
            {
                var services = provider?.Services?.Where(service => tokenDetails.ServiceIds.Contains(service.Id)).ToList();

                 if(services != null) 
                 {
                    if (services.All(x => x.RemovalTokenStatus == TokenStatusEnum.AdminCancelled)) { tokenStatus = TokenStatusEnum.AdminCancelled; }
                    else if (services.All(x => x.RemovalTokenStatus == TokenStatusEnum.RequestCompleted)) { tokenStatus = TokenStatusEnum.RequestCompleted; }
                    else if (services.All(x => x.RemovalTokenStatus == TokenStatusEnum.RequestResent)) { tokenStatus = TokenStatusEnum.RequestResent; }
                    else if (services.All(x => x.RemovalTokenStatus == TokenStatusEnum.UserCancelled)) { tokenStatus = TokenStatusEnum.UserCancelled; }
                }                   
            }           
            return tokenStatus;
        }

        public async Task<GenericResponse> UpdateRemovalStatus(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            string serviceNames = string.Join("\r", providerDto.Services.Select(s => s.ServiceName).ToList());
            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            List<string> cabEmails = await cabRepository.GetCabEmailListForProvider(providerDto.Id);

            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);

            bool tokenExists = !string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId);

            List<int>? serviceIdsToUpdate = tokenExists && removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0
                ? serviceIds
                : null;

            EventTypeEnum eventType = removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0?EventTypeEnum.RemoveServices2i : EventTypeEnum.RemoveProvider2i;

            genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, eventType, serviceIdsToUpdate, loggedInUserEmail);

            if (genericResponse.Success)
            {

                genericResponse = await removeProviderService.UpdateProviderStatus(providerDto.Id, loggedInUserEmail, eventType,team);
                if (tokenExists && eventType == EventTypeEnum.RemoveServices2i)
                {

                    var serviceRemovalReasonList = providerDto.Services.Where(s => s.ServiceRemovalReason.HasValue)
                  .Select(s => ServiceRemovalReasonEnumExtensions.GetDescription(s.ServiceRemovalReason.Value)).ToList();
                    string serviceRemovalReasons = string.Join("\r", serviceRemovalReasonList);


                    if (team == TeamEnum.Provider)
                    {                      
                        await emailSender.RemoveServiceConfirmationToProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, serviceNames, serviceRemovalReasons); //39/Provider/Confirmation of service removal request
                        await emailSender.RemoveServiceConfirmationToProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, serviceNames, serviceRemovalReasons);
                        await emailSender.ServiceRemovalConfirmationToDSIT(providerDto.RegisteredName, serviceNames, serviceRemovalReasons);//40/DSIT/Service removal request received
                    }
                    else if (team == TeamEnum.DSIT)
                    {
                        await emailSender.Service2iCheckApprovedToDSIT(serviceNames, serviceRemovalReasons); //53/DSIT/2i service removal check approved
                    }


                    await Task.Delay(500);
                    foreach(var cabEmail in cabEmails)
                    {
                        await emailSender.ServiceRemovedConfirmedToCabOrProvider(cabEmail, cabEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                    }                   
                    await emailSender.ServiceRemovedToDSIT(serviceNames, serviceRemovalReasons); //42/DSIT/Service removed
                    await emailSender.ServiceRemovedConfirmedToCabOrProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                    await emailSender.ServiceRemovedConfirmedToCabOrProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, serviceNames, serviceRemovalReasons);//41/CAB + Provider/Service removed
                }
                else
                {
                    // provider removal confirmations

                    string removalReason = providerDto.RemovalReason!=null? RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value):string.Empty;
                    if (team == TeamEnum.Provider)
                    {
                        await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);//31/Provider/Removal request confirmed
                        await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                        await emailSender.SendProviderRemovalConfirmationToDSIT(providerDto.RegisteredName, serviceNames);//32/DSIT/Provider removal confirmation
                    }
                    else if (team == TeamEnum.DSIT)
                    {
                        await emailSender._2iCheckApprovedNotificationToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                    }

                    await Task.Delay(500);
                    foreach (var cabEmail in cabEmails)
                    {
                        await emailSender.RecordRemovedConfirmedToCabOrProvider(cabEmail, cabEmail, providerDto.RegisteredName, serviceNames, removalReason);
                    }
                    await emailSender.SendRecordRemovedToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                    await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail, providerDto.RegisteredName, serviceNames, removalReason);
                    await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, providerDto.RegisteredName, serviceNames, removalReason);
                }
               
            }

            return genericResponse;
        }

    public async Task<GenericResponse> CancelRemovalRequest(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            string serviceNames = string.Join("\r", providerDto.Services.Select(s => s.ServiceName));

            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            EventTypeEnum eventType = removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0 ? 
                EventTypeEnum.RemoveServices2i : EventTypeEnum.RemoveProvider2i;

            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId)) //proceed update status if token exists
            {
                genericResponse = await removeProvider2iRepository.CancelServiceRemoval(providerDto.Id, team, eventType, serviceIds, loggedInUserEmail);
                genericResponse = await removeProviderService.UpdateProviderStatus(providerDto.Id, loggedInUserEmail, eventType, team);

                if (genericResponse.Success)
                {
                    if (team == TeamEnum.Provider)
                    {
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);//33/Provider/Removal request declined
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                        await emailSender.RemovalRequestDeclinedToDSIT(providerDto.RegisteredName, serviceNames); //34 / DSIT / Removal request declined by provider
                    }
                    else if (team == TeamEnum.DSIT && eventType == EventTypeEnum.RemoveServices2i)
                    {
                        await emailSender.Service2iCheckDeclinedToDSIT(serviceNames);//54/DSIT/2i check declined - service
                    }
                    else 
                    {
                        string removalReason = RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value);
                        await emailSender._2iCheckDeclinedNotificationToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                    }
                }
            }
            return genericResponse;
        }

        public async Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail)
        {
            return await removeProvider2iRepository.RemoveRemovalToken(token, tokenId, loggedInUserEmail);
        }

       

        
    }
}
