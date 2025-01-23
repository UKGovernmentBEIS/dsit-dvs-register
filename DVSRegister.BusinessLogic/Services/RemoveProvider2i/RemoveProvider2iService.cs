using AutoMapper;
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
            RemoveProviderToken removeProviderToken = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            
            
          
            if (!string.IsNullOrEmpty(removeProviderToken.Token) && !string.IsNullOrEmpty(removeProviderToken.TokenId))   //proceed update status if token exists           
            {
                if (removeProviderToken.RemoveTokenServiceMapping != null && removeProviderToken.RemoveTokenServiceMapping.Count > 0) // remove selected services in this case
                {
                    List<int> serviceIds = providerDto.Services.Select(s => s.Id).ToList();
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, serviceIds, loggedInUserEmail);
                    // get updated service list and decide provider status
                    ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerDto.Id);
                    // update provider status
                    ProviderStatusEnum providerStatus = GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                    genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerDto.Id, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);

                }
                else
                {                    
                    genericResponse = await removeProvider2iRepository.UpdateRemovalStatus(providerDto.Id, team, EventTypeEnum.RemoveServices2i, null, loggedInUserEmail);
                }
               
                if (genericResponse.Success)
                {
                    if(team == TeamEnum.Provider) 
                    {
                        await emailSender.SendRemovalRequestConfirmedToDIP(providerDto.PrimaryContactFullName, providerDto.PrimaryContactEmail);
                    }

                    if (team == TeamEnum.DSIT)
                    {
                       //to do
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
