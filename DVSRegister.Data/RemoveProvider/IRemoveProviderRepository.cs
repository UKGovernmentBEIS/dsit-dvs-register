using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRemoveProviderRepository
    {
        public Task<GenericResponse> UpdateProviderStatus(int providerProfileId, ProviderStatusEnum providerStatus, string loggedInUserEmail, EventTypeEnum eventType,TeamEnum team);
        public Task<ProviderProfile> GetProviderWithAllServices(int providerId);
    }
}
