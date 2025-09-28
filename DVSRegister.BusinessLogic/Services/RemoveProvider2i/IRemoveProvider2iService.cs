using DVSRegister.BusinessLogic.Models.Remove2i;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface IRemoveProvider2iService
    {
        //Remove provider
        public Task<ProviderRemovalRequestDto?> GetProviderRemovalDetailsByRemovalToken(string token, string tokenId);
        public Task<GenericResponse> ApproveProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail);        public Task<GenericResponse> CancelProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail);

        public Task<TokenStatusEnum> GetTokenStatus(TokenDetails tokenDetails);

        //Remove service
        public Task<ServiceRemovalRequestDto?> GetServiceRemovalDetailsByRemovalToken(string token, string tokenId);
        public Task<GenericResponse> ApproveServiceRemoval(ServiceRemovalRequestDto serviceRemovalRequest, string loggedInUserEmail);
        public Task<GenericResponse> CancelServiceRemoval(ServiceRemovalRequestDto serviceRemovalRequest, string loggedInUserEmail);
    }
}
