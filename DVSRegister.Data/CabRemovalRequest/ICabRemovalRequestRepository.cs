using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Data.CabRemovalRequest
{
    public interface ICabRemovalRequestRepository 
    {       
        public Task<GenericResponse> AddServiceRemovalRequest(int cabId, int serviceId, string loggedInUserEmail, string removalReasonByCab);
        public Task<bool> IsLastService(int serviceId, int providerProfileId);
    }
}
