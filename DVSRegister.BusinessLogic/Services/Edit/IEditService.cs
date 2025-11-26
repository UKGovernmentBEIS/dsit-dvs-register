using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.Edit
{
    public interface IEditService
    {
        public Task<GenericResponse> SaveProviderDraft(ProviderProfileDraftDto draftDto, string loggedInUserEmail, int cabId);
        public Task<ProviderProfileDto> GetProviderDetails(int providerId, int cabId);
        public Task<(Dictionary<string, List<string>>, Dictionary<string, List<string>>)> GetProviderKeyValue(ProviderProfileDraftDto changedProvider, ProviderProfileDto currentProvider);
        public Task ConfirmPrimaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName, string providerName);
        public Task ConfirmSecondaryContactUpdates(Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, string emailAddress, string recipientName, string providerName);
        public Task<GenericResponse> UpdateCompanyInfoAndPublicProviderInfo(ProviderProfileDto providerProfileDto, string loggedInUserEmail);      
        public Task<GenericResponse> UpdatePrimaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail);
        public Task<GenericResponse> UpdateSecondaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail);    
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPublicContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData);
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetSecondaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData);
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPrimaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData);
        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetCompanyValueUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData);
    }
}
