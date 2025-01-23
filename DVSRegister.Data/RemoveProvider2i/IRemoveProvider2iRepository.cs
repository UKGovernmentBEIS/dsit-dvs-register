using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRemoveProvider2iRepository
    {
        #region Remove Provider

        public Task<RemoveProviderToken> GetRemoveProviderToken(string token, string tokenId);

        public Task<ProviderProfile> GetProviderDetails(int providerId);
        public Task<GenericResponse> UpdateRemovalStatus(int providerProfileId, TeamEnum teamEnum, EventTypeEnum eventType, List<int>? serviceIds, string loggedInUserEmail);
        public Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail);

        public Task<ProviderProfile> GetProviderWithAllServices(int providerId);
        public Task<GenericResponse> UpdateProviderStatus(int providerProfileId, ProviderStatusEnum providerStatus, string loggedInUserEmail, EventTypeEnum eventType);
        #endregion
    }
}
