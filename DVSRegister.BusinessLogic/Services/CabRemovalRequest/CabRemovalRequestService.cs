using AutoMapper;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class CabRemovalRequestService : ICabRemovalRequestService
    {

        private readonly ICabRemovalRequestRepository cabRemovalRequestRepository;
        private readonly IRemoveProvider2iRepository removeProvider2iRepository;
    
        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository, IRemoveProvider2iRepository removeProvider2iRepository)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;  
            this.removeProvider2iRepository = removeProvider2iRepository;
        }

        public async Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabId, providerProfileId,serviceId,loggedInUserEmail, removalReasonByCab);

            if(genericResponse.Success) 
            {
                // get updated service list and decide provider status
                ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerProfileId);
                // update provider status
                ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerProfileId, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);
            }
         
            return genericResponse;
        }
    }
}
