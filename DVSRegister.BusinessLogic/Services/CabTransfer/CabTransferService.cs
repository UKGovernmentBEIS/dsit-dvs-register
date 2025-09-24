using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.CabTransfer;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services.CabTransfer
{
    public class CabTransferService: ICabTransferService
    {
        private readonly ICabTransferRepository cabTransferRepository;
        private readonly IRemoveProviderService removeProviderService;
        private readonly CabTransferEmailSender emailSender;
        private readonly IMapper automapper;

        public CabTransferService(ICabTransferRepository cabTransferRepository, IRemoveProviderService removeProviderService, CabTransferEmailSender emailSender, IMapper automapper)
        {
            this.cabTransferRepository = cabTransferRepository;
            this.removeProviderService = removeProviderService;
            this.emailSender = emailSender;
            this.automapper = automapper;            
        }


        public async Task<List<CabTransferRequestDto>> GetServiceTransferRequests(int cabId)
        {
            var list = await cabTransferRepository.GetServiceTransferRequests(cabId);
            return automapper.Map<List<CabTransferRequestDto>>(list);
           
        }

        public async Task<ServiceDto> GetServiceDetailsWithCabTransferDetails(int serviceId, int cabId)
        {
            var service = await cabTransferRepository.GetServiceDetailsWithCabTransferDetails(serviceId, cabId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }
        public async Task<CabTransferRequestDto> GetCabTransferRequestDetails(int requestId)
        {
            var requestDetails = await cabTransferRepository.GetCabTransferRequestDetails(requestId);
            return automapper.Map<CabTransferRequestDto>(requestDetails);

        }

        public async Task<GenericResponse> ApproveOrCancelTransferRequest(bool approve, int requestId, int providerProfileId, string loggedInUserEmail)
        {            
            GenericResponse genericResponse = await cabTransferRepository.ApproveOrCancelTransferRequest(approve,requestId,providerProfileId, loggedInUserEmail);

            if (genericResponse.Success)
            {               

                genericResponse = await removeProviderService.UpdateProviderStatus(providerProfileId, loggedInUserEmail, EventTypeEnum.ApproveOrRejectReAssign, TeamEnum.CAB);

                if (!genericResponse.Success)
                    return genericResponse;

                var fullRequest = await cabTransferRepository.GetCabTransferRequestDetails(requestId);
                string serviceName = fullRequest.Service.ServiceName;
                string providerName = fullRequest.Service.Provider.RegisteredName;

                //Tranfered to or accepting cab list
                List<CabUser> activeCabBUsers = await cabTransferRepository.GetActiveCabUsers(fullRequest.ToCabId);
                var acceptingCabName = activeCabBUsers.FirstOrDefault()?.Cab.CabName ?? string.Empty;

                if (approve)
                {
                    foreach (var user in activeCabBUsers)
                    {
                        await emailSender.SendCabTransferConfirmationToCabB(user.CabEmail, acceptingCabName, providerName, serviceName);
                    }
                    await emailSender.SendCabTransferConfirmationToDSIT(fullRequest.FromCabUser.Cab.CabName, acceptingCabName, providerName, serviceName);
                    //Tranfered from  cab list
                    List<CabUser> activeCabAUsers = await cabTransferRepository.GetActiveCabUsers(fullRequest.FromCabUser.CabId);
                    var currentCabName = activeCabAUsers.FirstOrDefault()?.Cab.CabName ?? string.Empty;

                    foreach (var user in activeCabAUsers)
                    {
                        await emailSender.SendCabTransferConfirmationToCabA(user.CabEmail, currentCabName, acceptingCabName, providerName, serviceName);
                    }
                }
                else
                {
                    foreach (var user in activeCabBUsers)
                    {
                        await emailSender.SendCabTransferCancellationToCabB(user.CabEmail, acceptingCabName, providerName, serviceName);
                    }
                    await emailSender.SendCabTransferCancellationToDSIT(fullRequest.FromCabUser.Cab.CabName, acceptingCabName, providerName, serviceName);
                }
            }
            
            return genericResponse;
        }
    }
}
