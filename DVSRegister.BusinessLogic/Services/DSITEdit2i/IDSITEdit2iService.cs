using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IDSITEdit2iService
    {
        public Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId);
        public Task<GenericResponse> UpdateProviderAndServiceStatusAndData(ProviderProfileDraftDto providerProfileDraft);
        public Task<bool> RemoveProviderDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelProviderUpdates(ProviderProfileDraftDto providerProfileDraft);

        public Task<ServiceDraftTokenDto> GetServiceChangesByToken(string token, string tokenId);
        public Task<GenericResponse> UpdateServiceStatusAndData(ServiceDraftDto serviceDraft);
        public Task<bool> RemoveServiceDraftToken(string token, string tokenId);
        public Task<GenericResponse> CancelServiceUpdates(ServiceDraftDto serviceDraft);

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetServiceKeyValue(ServiceDraftDto currentData, ServiceDto previousData);
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetProviderKeyValue(ProviderProfileDraftDto currentData, ProviderProfileDto previousData);
    }
}
