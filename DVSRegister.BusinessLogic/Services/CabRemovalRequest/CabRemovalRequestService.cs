using AutoMapper;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabRemovalRequest;

namespace DVSRegister.BusinessLogic.Services
{
    public class CabRemovalRequestService : ICabRemovalRequestService
    {

        private readonly ICabRemovalRequestRepository cabRemovalRequestRepository;
    
        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository, IMapper automapper)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;            
        }

        public async Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabId, providerProfileId,serviceId,loggedInUserEmail, removalReasonByCab);
            return genericResponse;
        }
    }
}
