using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data.Edit
{
    public interface IEditRepository
    {
        public Task<GenericResponse> SaveProviderDraft(ProviderProfileDraft draft, string loggedInUserEmail);
        public Task<ProviderProfile> GetProviderDetails(int providerId, int cabId);
        public Task<GenericResponse> UpdateCompanyInfoAndPublicProviderInfo(ProviderProfile providerProfile, string loggedInUserEmail);    
        public Task<GenericResponse> UpdatePrimaryContact(ProviderProfile providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> UpdateSecondaryContact(ProviderProfile providerProfile, string loggedInUserEmail);      
    }
}
