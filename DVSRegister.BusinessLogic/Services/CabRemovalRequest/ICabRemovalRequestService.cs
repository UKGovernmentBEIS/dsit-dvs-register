using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services
{
    public interface ICabRemovalRequestService
    {
        public Task <GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab);
    }
}
