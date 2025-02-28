using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IDSITEdit2iService
    {
        public Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId);
        public Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId);
        public Task<bool> RemoveProviderDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId);

        public Task<ServiceDraftTokenDto> GetServiceChangesByToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceStatusAndData(int serviceId, int serviceDraftId);
        public Task<bool> RemoveServiceDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelServiceUpdates(int serviceId, int serviceDraftId);

        public Dictionary<string, List<string>> GetPreviousDataKeyPair(ServiceDraftDto currentData, ServiceDto previousData);
        public Dictionary<string, List<string>> GetCurrentDataKeyPair(ServiceDraftDto currentData);
    }
}
