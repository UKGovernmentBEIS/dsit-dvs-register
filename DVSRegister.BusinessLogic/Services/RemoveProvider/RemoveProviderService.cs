using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class RemoveProviderService(IRemoveProviderRepository removeProviderRepository) :IRemoveProviderService
    {
        private readonly IRemoveProviderRepository removeProviderRepository = removeProviderRepository;      

        public async Task<GenericResponse> UpdateProviderStatus(int providerProfileId, string loggedInUserEmail, EventTypeEnum eventType, TeamEnum team)
        {

            ProviderProfile providerProfile = await removeProviderRepository.GetProviderWithAllServices(providerProfileId);
            ProviderStatusEnum providerStatus = RepositoryHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
            return await removeProviderRepository.UpdateProviderStatus(providerProfileId, providerStatus, loggedInUserEmail, eventType, team);
        }


       
    }
}
