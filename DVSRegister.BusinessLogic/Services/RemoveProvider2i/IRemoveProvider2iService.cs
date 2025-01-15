using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRemoveProvider2iService
    {
        //Remove provider
        public Task<ProviderProfileDto?> GetProviderAndServiceDetailsByRemovalToken(string token, string tokenId);
        public Task<GenericResponse> UpdateRemovalStatus(string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail);
        public Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail);
    }
}
