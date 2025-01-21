using DVSRegister.CommonUtility.Models;

namespace DVSRegister.Data.CabRemovalRequest
{
    public interface ICabRemovalRequestRepository 
    {
        public Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab);
    }
}
