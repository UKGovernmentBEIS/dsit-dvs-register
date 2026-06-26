using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabRemovalRequest;

namespace DVSRegister.BusinessLogic.Services
{
    public class CabRemovalRequestService : ICabRemovalRequestService
    {
        private readonly ICabRemovalRequestRepository cabRemovalRequestRepository;
        private readonly ICabRemovalRequestEmailSender emailSender;
        private readonly ICabService cabService;

        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository,
            ICabService cabService,
            ICabRemovalRequestEmailSender emailSender)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;
            this.emailSender = emailSender;
            this.cabService = cabService;
        }

        public async Task<GenericResponse> AddServiceRemovalrequest(int cabId, int serviceId, string loggedInUserEmail,
            string removalReasonByCab, bool isProviderRemoval)
        {
            var genericResponse =
                await cabRemovalRequestRepository.AddServiceRemovalRequest(cabId, serviceId, loggedInUserEmail,
                    removalReasonByCab);

            if (!genericResponse.Success)
                return genericResponse;

            var service = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);

            if (service == null)
            {
                return genericResponse;
            }

            if (isProviderRemoval)
            {
                await emailSender.RecordRemovalRequestByCabToDSIT(service.Provider.RegisteredName, service.ServiceName,
                    removalReasonByCab);
                await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail,
                    service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
            }
            else
            {
                await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail,
                    service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                await emailSender.CabServiceRemovalRequestedToDSIT(service.Provider.RegisteredName, service.ServiceName,
                    removalReasonByCab);
            }

            return genericResponse;
        }

        public async Task<bool> IsLastService(int serviceId, int providerProfileId)
        {
            return await cabRemovalRequestRepository.IsLastService(serviceId, providerProfileId);
        }

        public async Task<GenericResponse> CancelServiceRemovalRequest(int cabId, int serviceId,
            string loggedInUserEmail)
        {
            var genericResponse =
                await cabRemovalRequestRepository.CancelServiceRemovalRequest(serviceId, loggedInUserEmail);

            if (!genericResponse.Success)
                return genericResponse;

            var service = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);

            if (service?.CabUser?.Cab == null)
            {
                return genericResponse;
            }

            var cabName = service.CabUser.Cab.CabName;
            var providerName = service.Provider.RegisteredName;
            var serviceName = service.ServiceName;

            await emailSender.RemovalRequestCancelledToCab(loggedInUserEmail, cabName, providerName, serviceName);
            await emailSender.RemovalRequestCancelledToDSIT(cabName, providerName, serviceName);

            return genericResponse;
        }
    }
}