using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;

namespace DVSRegister.Data
{
    public interface IRemoveProvider2iRepository
    {
        #region Remove Provider

        public Task<ProviderRemovalRequest> GetRemoveProviderToken(string token, string tokenId);
        public Task<GenericResponse> ApproveProviderRemoval(int providerProfileId, int providerRemovalRequestId, string loggedInUserEmail);
        public Task<GenericResponse> CancelRemoveProviderRequest(int providerProfileId, int providerRemovalRequestId, string loggedInUserEmail);
        public Task<ProviderProfile> GetProviderDetails(int providerId);
        public Task<Service> GetServiceDetailsWithProvider(int serviceId);
        public Task<ServiceRemovalRequest> GetRemoveServiceToken(string token, string tokenId);
        public Task<Service> GetServiceDetails(int serviceId);
        public Task<GenericResponse> ApproveServiceRemoval(int serviceId, int serviceRemovalRequestId, string loggedInUserEmail);
        public Task<GenericResponse> CancelRemoveServiceRequest(int serviceId, int serviceRemovalRequestId, string loggedInUserEmail);


        #endregion
    }
}
