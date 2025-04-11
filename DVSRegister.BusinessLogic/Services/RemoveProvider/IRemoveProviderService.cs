using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRemoveProviderService
    {
        public Task<GenericResponse> UpdateProviderStatus(int providerProfileId, string loggedInUserEmail, EventTypeEnum eventType, TeamEnum team);
        public Task<GenericResponse> UpdateProviderStatus(ProviderProfile providerProfile, int providerProfileId, string loggedInUserEmail, EventTypeEnum eventType, TeamEnum team);
    }
}
