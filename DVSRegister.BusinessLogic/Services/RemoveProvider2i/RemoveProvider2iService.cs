using AutoMapper;
using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models.Remove2i;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services
{
    public class RemoveProvider2iService :IRemoveProvider2iService 
    {

        private readonly IRemoveProvider2iRepository removeProvider2iRepository;

        private readonly IMapper mapper;
        private readonly Removal2iCheckEmailSender emailSender;
        private readonly ICabRepository cabRepository;


        public RemoveProvider2iService(IRemoveProvider2iRepository removeProvider2iRepository, 
            ICabRepository cabRepository, IMapper mapper, Removal2iCheckEmailSender emailSender)
        {         
            this.removeProvider2iRepository = removeProvider2iRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.cabRepository = cabRepository;
        }

        #region Provider
        public async Task<ProviderRemovalRequestDto?> GetProviderRemovalDetailsByRemovalToken(string token, string tokenId)
        {
            ProviderRemovalRequest providerRemovalRequest = await removeProvider2iRepository.GetRemoveProviderToken(token, tokenId);
            ProviderProfile providerWithServiceDetails = await removeProvider2iRepository.GetProviderDetails(providerRemovalRequest.ProviderProfileId);
            providerRemovalRequest.Provider = providerWithServiceDetails;
             return mapper.Map<ProviderRemovalRequestDto>(providerRemovalRequest);           
        }
       
        public async Task<GenericResponse> ApproveProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            genericResponse = await removeProvider2iRepository.ApproveProviderRemoval(providerRemovalRequest.ProviderProfileId, providerRemovalRequest.Id, loggedInUserEmail);

            if (genericResponse.Success)
            {
                string serviceNames = string.Join("\r", providerRemovalRequest.Provider.Services.Where(s => s.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation).Select(s => s.ServiceName).ToList());
                List<int> serviceIds = providerRemovalRequest.Provider.Services.Select(s => s.Id).ToList();
                List<string> cabEmails = await cabRepository.GetCabEmailListForServices(serviceIds);
                string removalReason = RemovalReasonsEnumExtensions.GetDescription(providerRemovalRequest.RemovalReason);


                await emailSender.SendRemovalRequestConfirmedToDIP(providerRemovalRequest.Provider.PrimaryContactFullName, providerRemovalRequest.Provider.PrimaryContactEmail);//31/Provider/Removal request confirmed
                await emailSender.SendRemovalRequestConfirmedToDIP(providerRemovalRequest.Provider.SecondaryContactFullName, providerRemovalRequest.Provider.SecondaryContactEmail);
                await emailSender.SendProviderRemovalConfirmationToDSIT(providerRemovalRequest.Provider.RegisteredName, serviceNames);//32/DSIT/Provider removal confirmation

                await Task.Delay(500);
                foreach (var cabEmail in cabEmails)
                {
                    await emailSender.RecordRemovedConfirmedToCabOrProvider(cabEmail, cabEmail, providerRemovalRequest.Provider.RegisteredName, serviceNames, removalReason);
                }
                await emailSender.SendRecordRemovedToDSIT(providerRemovalRequest.Provider.RegisteredName, serviceNames, removalReason);
                await emailSender.RecordRemovedConfirmedToCabOrProvider(providerRemovalRequest.Provider.PrimaryContactFullName, providerRemovalRequest.Provider.PrimaryContactEmail, providerRemovalRequest.Provider.RegisteredName, serviceNames, removalReason);
                await emailSender.RecordRemovedConfirmedToCabOrProvider(providerRemovalRequest.Provider.SecondaryContactFullName, providerRemovalRequest.Provider.SecondaryContactEmail, providerRemovalRequest.Provider.RegisteredName, serviceNames, removalReason);

            }
            return genericResponse;
           }
        public async Task<GenericResponse> CancelProviderRemoval(ProviderRemovalRequestDto providerRemovalRequest, string loggedInUserEmail)
        {
            GenericResponse genericResponse = new();
            genericResponse = await removeProvider2iRepository.CancelRemoveProviderRequest(providerRemovalRequest.ProviderProfileId, providerRemovalRequest.Id, loggedInUserEmail);
            if(genericResponse.Success)
            {
                string serviceNames = string.Join("\r", providerRemovalRequest.Provider.Services.Select(s => s.ServiceName));
                await emailSender.RemovalRequestDeclinedToProvider(providerRemovalRequest.Provider.PrimaryContactFullName, providerRemovalRequest.Provider.PrimaryContactEmail);//33/Provider/Removal request declined
                await emailSender.RemovalRequestDeclinedToProvider(providerRemovalRequest.Provider.SecondaryContactFullName, providerRemovalRequest.Provider.SecondaryContactEmail);
                await emailSender.RemovalRequestDeclinedToDSIT(providerRemovalRequest.Provider.RegisteredName, serviceNames); //34 / DSIT / Removal request declined by provider
            }
            return genericResponse;

        }



        #endregion

        #region Service
        public async Task<ServiceRemovalRequestDto?> GetServiceRemovalDetailsByRemovalToken(string token, string tokenId)
        {
            ServiceRemovalRequest serviceRemovalRequest = await removeProvider2iRepository.GetRemoveServiceToken(token, tokenId);
            Service fullServiceDetails = await removeProvider2iRepository.GetServiceDetails(serviceRemovalRequest.Service.Id);
            serviceRemovalRequest.Service = fullServiceDetails;           
            return mapper.Map<ServiceRemovalRequestDto>(serviceRemovalRequest);
        }


        public async Task<GenericResponse> ApproveServiceRemoval(ServiceRemovalRequestDto serviceRemovalRequest, string loggedInUserEmail)
        {           
            GenericResponse genericResponse  = await removeProvider2iRepository.ApproveServiceRemoval(serviceRemovalRequest.ServiceId, serviceRemovalRequest.Id, loggedInUserEmail);
            if (genericResponse.Success)
            {
                string removalReason = ServiceRemovalReasonEnumExtensions.GetDescription(serviceRemovalRequest.ServiceRemovalReason.Value);
                await emailSender.RemoveServiceConfirmationToProvider(serviceRemovalRequest.Service.Provider.PrimaryContactFullName, serviceRemovalRequest.Service.Provider.PrimaryContactEmail, serviceRemovalRequest.Service.ServiceName, removalReason); //39/Provider/Confirmation of service removal 
                await emailSender.RemoveServiceConfirmationToProvider(serviceRemovalRequest.Service.Provider.SecondaryContactFullName, serviceRemovalRequest.Service.Provider.SecondaryContactEmail, serviceRemovalRequest.Service.ServiceName, removalReason);
                await emailSender.ServiceRemovalConfirmationToDSIT(serviceRemovalRequest.Service.Provider.RegisteredName, serviceRemovalRequest.Service.ServiceName, removalReason);//40/DSIT/Service removal request received
            }
            return genericResponse;
        }

        public async Task<GenericResponse> CancelServiceRemoval(ServiceRemovalRequestDto serviceRemovalRequest, string loggedInUserEmail)
        {

            GenericResponse genericResponse = await removeProvider2iRepository.CancelRemoveServiceRequest(serviceRemovalRequest.ServiceId, serviceRemovalRequest.Id, loggedInUserEmail);
            if (genericResponse.Success)
            {
                await emailSender.RemovalRequestDeclinedToProvider(serviceRemovalRequest.Service.Provider.PrimaryContactFullName, serviceRemovalRequest.Service.Provider.PrimaryContactEmail);//33/Provider/Removal request declined
                await emailSender.RemovalRequestDeclinedToProvider(serviceRemovalRequest.Service.Provider.SecondaryContactFullName, serviceRemovalRequest.Service.Provider.SecondaryContactEmail);
                await emailSender.RemovalRequestDeclinedToDSIT(serviceRemovalRequest.Service.Provider.RegisteredName, serviceRemovalRequest.Service.ServiceName); //34 / DSIT / Removal request declined by provider
            }
            return genericResponse;

        }
        #endregion

    }
}
