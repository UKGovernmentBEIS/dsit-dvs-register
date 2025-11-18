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
        public Task ConfirmPrimaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName, string providerName);
        public Task ConfirmSecondaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName, string providerName);
    }
}
