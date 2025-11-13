using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.Edit
{
    public interface IEditService
    {
        public Task<GenericResponse> SaveProviderDraft(ProviderProfileDraftDto draftDto, string loggedInUserEmail);
        public Task<ProviderProfileDto> GetProviderDetails(int providerId);
        public Task<(Dictionary<string, List<string>>, Dictionary<string, List<string>>)> GetProviderKeyValue(ProviderProfileDraftDto changedProvider, ProviderProfileDto currentProvider);
    }
}
