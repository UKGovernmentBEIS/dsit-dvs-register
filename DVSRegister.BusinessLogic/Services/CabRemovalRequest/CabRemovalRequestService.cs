using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class CabRemovalRequestService : ICabRemovalRequestService
    {

        private readonly ICabRemovalRequestRepository cabRemovalRequestRepository;
        private readonly IRemoveProviderService removeProviderService;
        private readonly IRemoveProviderRepository removeProviderRepository;
        private readonly CabRemovalRequestEmailSender emailSender;      
    
        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository,
            CabRemovalRequestEmailSender emailSender, IRemoveProviderService removeProviderService, IRemoveProviderRepository removeProviderRepository)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;  
            this.removeProviderService = removeProviderService;
            this.emailSender = emailSender;   
            this.removeProviderRepository = removeProviderRepository;
        }

        public async Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab, string whatToRemove)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabId, providerProfileId,serviceId,loggedInUserEmail, removalReasonByCab);

            if(genericResponse.Success) 
            {

                // get updated service list and decide provider status
                ProviderProfile providerProfile = await removeProviderRepository.GetProviderWithAllServices(providerProfileId);
               
                genericResponse = await removeProviderService.UpdateProviderStatus(providerProfile, providerProfileId, loggedInUserEmail, EventTypeEnum.RemoveServiceRequestedByCab, TeamEnum.CAB);

                if (whatToRemove == "provider")
                {
                    Service service = providerProfile.Services.FirstOrDefault();
                    if (service != null)
                    {
                        await emailSender.RecordRemovalRequestByCabToDSIT(providerProfile.RegisteredName, service.ServiceName, service.RemovalReasonByCab);
                        await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                    }
                }
                else if (whatToRemove == "service")
                {
  
                    Service service = providerProfile.Services.FirstOrDefault(s => s.Id == serviceId) ?? new Service();

                    await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                    await emailSender.CabServiceRemovalRequestedToDSIT(providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                }



            }

            return genericResponse;
        }
    }
}
