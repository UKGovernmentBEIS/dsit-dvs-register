using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.CabRemovalRequest;

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

        public async Task<GenericResponse> AddServiceRemovalrequest(int cabId,  int serviceId, string loggedInUserEmail, string removalReasonByCab, string whatToRemove)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.AddServiceRemovalRequest(cabId,  serviceId, loggedInUserEmail, removalReasonByCab);
            if(genericResponse.Success)
            {
                var service = await cabService.GetServiceDetailsWithProvider(serviceId, cabId);            

                if (whatToRemove == "provider")
                {
                 
                    if (service != null)
                    {
                        await emailSender.RecordRemovalRequestByCabToDSIT(service.Provider.RegisteredName, service.ServiceName, service.RemovalReasonByCab);
                        await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail, service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                    }
                }
                else if (whatToRemove == "service")
                {                    

                    await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail, service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                    await emailSender.CabServiceRemovalRequestedToDSIT(service.Provider.RegisteredName, service.ServiceName, removalReasonByCab);
                }               
            }
            return genericResponse;
        }
        //public async Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab, string whatToRemove)
        //{
        //    GenericResponse genericResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabId, providerProfileId,serviceId,loggedInUserEmail, removalReasonByCab);

        //    if(genericResponse.Success) 
        //    {

        
        //        ProviderProfile providerProfile = await removeProviderRepository.GetProviderWithAllServices(providerProfileId);               
               

        //        if (whatToRemove == "provider")
        //        {
        //            Service service = providerProfile.Services.FirstOrDefault();
        //            if (service != null)
        //            {
        //                await emailSender.RecordRemovalRequestByCabToDSIT(providerProfile.RegisteredName, service.ServiceName, service.RemovalReasonByCab);
        //                await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
        //            }
        //        }
        //        else if (whatToRemove == "service")
        //        {
  
        //            Service service = providerProfile.Services.FirstOrDefault(s => s.Id == serviceId) ?? new Service();

        //            await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
        //            await emailSender.CabServiceRemovalRequestedToDSIT(providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
        //        }



        //    }

        //    return genericResponse;
        //}
    }
}
