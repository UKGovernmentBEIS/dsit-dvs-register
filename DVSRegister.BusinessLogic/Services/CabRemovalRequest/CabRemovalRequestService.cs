using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabRemovalRequest;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class CabRemovalRequestService : ICabRemovalRequestService
    {

        private readonly ICabRemovalRequestRepository cabRemovalRequestRepository;
        private readonly CabRemovalRequestEmailSender emailSender;
        private readonly ICabService cabService;

        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository, ICabService cabService,
        CabRemovalRequestEmailSender emailSender)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;             
            this.emailSender = emailSender;
            this.cabService = cabService;
        }

        public async Task<GenericResponse> AddServiceRemovalrequest(int cabId,  int serviceId, string loggedInUserEmail, string removalReasonByCab, bool isProviderRemoval)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.AddServiceRemovalRequest(cabId,  serviceId, loggedInUserEmail, removalReasonByCab);
            if(genericResponse.Success)
            {
                var service = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);            

                if (isProviderRemoval)
                {
                 
                    if (service != null)
                    {
                        await emailSender.RecordRemovalRequestByCabToDSIT(service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                        await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail, service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                    }
                }
                else
                {                    

                    await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail, service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                    await emailSender.CabServiceRemovalRequestedToDSIT(service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                }               
            }
            return genericResponse;
        }
        public async Task<bool> IsLastService(int serviceId, int providerProfileId)
        {
            return await cabRemovalRequestRepository.IsLastService(serviceId, providerProfileId);
        }

        public async Task<GenericResponse> CancelServiceRemovalRequest(int cabId, int serviceId, string loggedInUserEmail)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.CancelServiceRemovalRequest(serviceId, loggedInUserEmail);
            if (genericResponse.Success)
            {
                var service = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);
                await emailSender.RemovalRequestCancelledToCab(loggedInUserEmail, service.CabUser.Cab.CabName, service.Provider.RegisteredName, service.ServiceName);
                await emailSender.RemovalRequestCancelledToDSIT(service.CabUser.Cab.CabName, service.Provider.RegisteredName, service.ServiceName);
            }
            return genericResponse;
        }
    }
}
