using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
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
        private readonly IRemoveProvider2iRepository removeProvider2iRepository;
        private readonly IEmailSender emailSender;      
    
        public CabRemovalRequestService(ICabRemovalRequestRepository cabRemovalRequestRepository, IRemoveProvider2iRepository removeProvider2iRepository, 
            IEmailSender emailSender)
        {
            this.cabRemovalRequestRepository = cabRemovalRequestRepository;  
            this.removeProvider2iRepository = removeProvider2iRepository;
            this.emailSender = emailSender;           
        }

        public async Task<GenericResponse> UpdateRemovalStatus(int cabId, int providerProfileId, int serviceId, string loggedInUserEmail, string removalReasonByCab)
        {
            GenericResponse genericResponse = await cabRemovalRequestRepository.UpdateRemovalStatus(cabId, providerProfileId,serviceId,loggedInUserEmail, removalReasonByCab);

            if(genericResponse.Success) 
            {
                // get updated service list and decide provider status
                ProviderProfile providerProfile = await removeProvider2iRepository.GetProviderWithAllServices(providerProfileId);
                // update provider status
                ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                genericResponse = await removeProvider2iRepository.UpdateProviderStatus(providerProfileId, providerStatus, loggedInUserEmail, EventTypeEnum.RemoveProvider2i);
               
                if (providerProfile.Services.Count == 1 && providerStatus == ProviderStatusEnum.CabAwaitingRemovalConfirmation)// remove provider request
                {
                    Service service = providerProfile.Services.ToList()[0];
                    await emailSender.RecordRemovalRequestByCabToDSIT(providerProfile.RegisteredName, service.ServiceName, service.RemovalReasonByCab);
                    await emailSender.RecordRemovalRequestConfirmationToCab(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                }
                else // remove service request
                {
                    Service service = providerProfile.Services.Where(s => s.Id == serviceId).FirstOrDefault()??new Service();
                    //45 / CAB / Service removal requested
                    await emailSender.CabServiceRemovalRequested(loggedInUserEmail, loggedInUserEmail, providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                    //46/DSIT/CAB service removal request
                    await emailSender.CabServiceRemovalRequestedToDSIT(providerProfile.RegisteredName, service.ServiceName, removalReasonByCab);
                }

               
            }
         
            return genericResponse;
        }
    }
}
