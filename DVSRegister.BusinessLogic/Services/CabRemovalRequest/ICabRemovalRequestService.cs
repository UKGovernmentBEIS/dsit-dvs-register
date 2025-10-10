using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ICabRemovalRequestService
    {
        public Task<GenericResponse> AddServiceRemovalrequest(int cabId, int serviceId, string loggedInUserEmail, string removalReasonByCab, bool isProviderRemoval);
        public Task<GenericResponse> CancelServiceRemovalRequest(int cabId, int serviceId, string loggedInUserEmail);
        public Task<bool> IsLastService(int serviceId, int providerProfileId);


    }
}
