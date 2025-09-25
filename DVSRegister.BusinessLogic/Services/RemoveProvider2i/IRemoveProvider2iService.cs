using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRemoveProvider2iService
    {
        //Remove provider
        public Task<ProviderRemovalRequestDto?> GetProviderRemovalDetailsByRemovalToken(string token, string tokenId);
        public Task<GenericResponse> ApproveProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail);

        public Task<GenericResponse> CancelProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail);

       // public Task<GenericResponse> UpdateRemovalStatus(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail);
       // public Task<bool> RemoveRemovalToken(string token, string tokenId, string loggedInUserEmail);
      //  public Task<GenericResponse> CancelRemovalRequest(TeamEnum team, string token, string tokenId, ProviderProfileDto providerDto, string loggedInUserEmail);
        public Task<TokenStatusEnum> GetTokenStatus(TokenDetails tokenDetails);       
    }
}
