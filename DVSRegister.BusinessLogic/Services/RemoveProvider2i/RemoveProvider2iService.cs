using AutoMapper;
using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class RemoveProvider2iService :IRemoveProvider2iService 
    {

        private readonly IRemoveProvider2iRepository removeProvider2iRepository;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;


        public RemoveProvider2iService(IRemoveProvider2iRepository removeProvider2iRepository, IMapper mapper, IEmailSender emailSender)
        {
            this.removeProvider2iRepository = removeProvider2iRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
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
                    providerProfileDto.Services = providerProfileDto.Services.Where(service => mappedServiceIds.Contains(service.Id)).ToList();
                    return providerProfileDto;

                }
                else
                {
                    ProviderProfileDto providerProfileDto = mapper.Map<ProviderProfileDto>(provider);                    
                    return providerProfileDto;
                }
             
            }
            else
            {
                return null;
            }
        }

        public async Task<GenericResponse> UpdateRemovalStatus(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            string serviceNames = string.Empty;
            string removalReason = RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value);

            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            var filteredServiceNames = providerDto.Services.Select(s => s.ServiceName).ToList();
            serviceNames = string.Join("\r", filteredServiceNames);


            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {                   
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    // get updated service list and decide provider status
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    // update provider status
                    ProviderStatusEnum providerStatus = GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                }
                else
                {                 
                    //remove provider
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, null, loggedInUserEmail);


                    if (genericResponse.Success)
                    {
                        if (team == TeamEnum.Provider) // if provider confirms
                        {
                            await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);
                            await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                            await emailSender.SendProviderRemovalConfirmationToDSIT(providerDto.RegisteredName, serviceNames);

                            await Task.Delay(1000);
                            await emailSender.SendRecordRemovedToDSIT(providerDto.RegisteredName, serviceNames, removalReason);
                            await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail,providerDto.RegisteredName, serviceNames, removalReason);
                            await emailSender.RecordRemovedConfirmedToCabOrProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail, providerDto.RegisteredName, serviceNames, removalReason);
                        }

                        else if (team == TeamEnum.DSIT) //if dsit confirms 2i check
                        {                            
                            await emailSender._2iCheckApprovedNotificationToDSIT(providerDto.RegisteredName, serviceNames, removalReason);

                        }
                    }
                }
               
               

            }
            return genericResponse;
        }


        public async Task<GenericResponse> CancelRemovalRequest(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            string serviceNames = string.Empty;
            string removalReason = string.Empty;

            List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
            var filteredServiceNames = providerDto.Services.Select(s => s.ServiceName).ToList();
            serviceNames = string.Join("\r", filteredServiceNames);


            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {
                   
                    genericResponse = await removeProvider2iRepository.CancelServiceRemoval(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    // get updated service list and decide provider status
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    // update provider status
                    ProviderStatusEnum providerStatus = GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                }
                else
                {
                    removalReason = RemovalReasonsEnumExtensions.GetDescription(providerDto.RemovalReason.Value);
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    serviceIds = providerProfile.Services.Select(s => s.Id).ToList(); // for all services
                                                                                    
                    genericResponse = await removeProvider2iRepository.CancelServiceRemoval(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    ProviderStatusEnum providerStatus = GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                    if (team == TeamEnum.Provider)
                    {
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);
                        await emailSender.RemovalRequestDeclinedToProvider(providerDto.SecondaryContactFullName, providerDto.SecondaryContactEmail);
                        await emailSender.RemovalRequestDeclinedToDSIT(providerDto.RegisteredName, serviceNames);
                    }
                    else if (team == TeamEnum.DSIT)
                    {
                     

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

        #region private methods
        private ProviderStatusEnum GetProviderStatus(ICollection<Service> services, ProviderStatusEnum currentStatus)
        {
            ProviderStatusEnum providerStatus = currentStatus;
            if (services != null && services.Count > 0)
            {

                if (services.All(service => service.ServiceStatus == ServiceStatusEnum.Removed))
                {
                    providerStatus = ProviderStatusEnum.RemovedFromRegister;
                }

                var priorityOrder = new List<ServiceStatusEnum>
                    {
                        ServiceStatusEnum.CabAwaitingRemovalConfirmation,
                        ServiceStatusEnum.ReadyToPublish,
                        ServiceStatusEnum.AwaitingRemovalConfirmation,
                        ServiceStatusEnum.Published,
                        ServiceStatusEnum.Removed
                    };

                ServiceStatusEnum highestPriorityStatus = services.Select(service => service.ServiceStatus).OrderBy(status => priorityOrder.IndexOf(status)).FirstOrDefault();


                switch (highestPriorityStatus)
                {
                    case ServiceStatusEnum.CabAwaitingRemovalConfirmation:
                        return ProviderStatusEnum.CabAwaitingRemovalConfirmation;
                    case ServiceStatusEnum.ReadyToPublish:
                        bool hasPublishedServices = services.Any(service => service.ServiceStatus == ServiceStatusEnum.Published);
                        return hasPublishedServices ? ProviderStatusEnum.PublishedActionRequired : ProviderStatusEnum.ActionRequired;
                    case ServiceStatusEnum.AwaitingRemovalConfirmation:
                        return ProviderStatusEnum.AwaitingRemovalConfirmation;
                    case ServiceStatusEnum.Published:
                        return ProviderStatusEnum.Published;
                    default:
                        return ProviderStatusEnum.AwaitingRemovalConfirmation;
                }

            }
            return providerStatus;
        }
        #endregion
    }
}
