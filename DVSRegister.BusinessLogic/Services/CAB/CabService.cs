using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.CAB;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Services.CAB
{
    public class CabService : ICabService
    {
        private readonly ICabRepository cabRepository;
        private readonly IMapper automapper;
        private readonly IBucketService bucketService;


        public CabService(ICabRepository cabRepository, IMapper automapper, IBucketService bucketService)
        {
            this.cabRepository = cabRepository;
            this.automapper = automapper;
            this.bucketService = bucketService;
        }
     
        public async Task<List<RoleDto>> GetRoles()
        {
            var list = await cabRepository.GetRoles();
            return automapper.Map<List<RoleDto>>(list);
        }

        public async Task<List<SupplementarySchemeDto>> GetSupplementarySchemes()
        {
            var list = await cabRepository.GetSupplementarySchemes();
            return automapper.Map<List<SupplementarySchemeDto>>(list);
        }
        public async Task<List<IdentityProfileDto>> GetIdentityProfiles()
        {
            var list = await cabRepository.GetIdentityProfiles();
            return automapper.Map<List<IdentityProfileDto>>(list);
        }      

        public bool CheckCompanyInfoEditable(ProviderProfileDto providerProfileDto)
        {
            return providerProfileDto.Services == null || providerProfileDto.Services.Count==0 || // services not added ie certificate info not submitted yet
            providerProfileDto.Services.All(service => service.CertificateReview == null && service.ServiceStatus == ServiceStatusEnum.Submitted) || //certificate info submitted but review not started
            providerProfileDto.Services.All(service => service.CertificateReview == null
            || service.CertificateReview.CertificateReviewStatus != CertificateReviewEnum.Approved); //none of the services has an Approved status;
        }
        public async Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId=0)
        {
            if(providerId >0) 
            {
                return await cabRepository.CheckProviderRegisteredNameExists(registeredName,providerId);
            }
            else
            {
                return await cabRepository.CheckProviderRegisteredNameExists(registeredName);
            }
           
        }

        public async Task<List<ProviderProfileDto>> GetProviders(int cabId, string searchText = "")
        {
            var list = await cabRepository.GetProviders(cabId, searchText);
            List<ProviderProfileDto> providerDtos = automapper.Map<List<ProviderProfileDto>>(list);
            return providerDtos;
        }

        public async Task<ProviderProfileDto> GetProvider(int providerId, int cabId)
        {
            var provider = await cabRepository.GetProvider(providerId, cabId);
            ProviderProfileDto providerDto = automapper.Map<ProviderProfileDto>(provider);
            return providerDto;
        }
        public async Task<ServiceDto> GetServiceDetails(int serviceId, int cabId)
        {
            var service = await cabRepository.GetServiceDetails(serviceId, cabId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }

        public async Task<List<ServiceDto>> GetServiceList(int serviceKey, int cabId)
        {
            var serviceList = await cabRepository.GetServiceList(serviceKey, cabId);
            return automapper.Map<List<ServiceDto>>(serviceList);
 
        }

        public async Task<ServiceDto> GetServiceDetailsWithProvider(int serviceId, int cabId)
        {
            var service = await cabRepository.GetServiceDetailsWithProvider(serviceId, cabId);
            ServiceDto serviceDto = automapper.Map<ServiceDto>(service);
            return serviceDto;
        }
        public async Task<List<QualityLevelDto>> GetQualitylevels()
        {
            var list = await cabRepository.QualityLevels();
            return automapper.Map<List<QualityLevelDto>>(list);
        }
        public async Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId)
        {
            return await cabRepository.CheckValidCabAndProviderProfile(providerId, cabId);

        }

        public async Task<(bool, List<CabTransferRequestDto>)> GetPendingReassignRequests(int cabId)
        {
            var(pendingRequestCount, requestList) = await cabRepository.GetPendingReassignRequestsCount(cabId);
            return (pendingRequestCount>0?true:false, automapper.Map<List<CabTransferRequestDto>>(requestList));
        }

        #region Save/ update
        public async Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.SaveProviderProfile(providerProfile, loggedInUserEmail);
            return genericResponse;
        }
        public async Task<GenericResponse> SaveService(ServiceDto serviceDto, string loggedInUserEmail)
        {
            Service service = new ();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveService(service, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> SaveServiceReApplication(ServiceDto serviceDto, string loggedInUserEmail)
        {
            Service service = new ();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveServiceReApplication(service, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> SaveServiceAmendments(ServiceDto serviceDto, string existingFileLink, int existingServiceCabId, int cabId, string loggedInUserEmail)
        {          
            

            Service service = new();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveServiceAmendments(service, loggedInUserEmail);
            if(genericResponse.Success && CanDeleteCertificate(serviceDto.FileLink, existingFileLink, existingServiceCabId, cabId))
            {      
                //ToDo: un comment after s3 changes
               // await bucketService.DeleteFromS3Bucket(existingFileLink);
            }
            return genericResponse;
        }

        public bool CanDeleteCertificate(string currentFileLink,string existingFileLink, int existingServiceCabId, int cabId)
        {
            bool canDelete = false;
          
            if (existingServiceCabId != cabId)
                throw new InvalidOperationException(string.Format("Invalid CabId, Cab Id in Service  {0}, Cab Id in Session {1}",
                  existingServiceCabId, cabId));
            if (!string.IsNullOrEmpty(existingFileLink) && !string.IsNullOrEmpty(currentFileLink) && existingFileLink != currentFileLink)
            {
               canDelete = true;
            }
            return canDelete;
        }

        public async Task<GenericResponse> UpdateCompanyInfo(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.UpdateCompanyInfo(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> UpdatePrimaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.UpdatePrimaryContact(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> UpdateSecondaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.UpdateSecondaryContact(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

        public async Task<GenericResponse> UpdatePublicProviderInformation(ProviderProfileDto providerProfileDto, string loggedInUserEmail)
        {
            ProviderProfile providerProfile = new();
            automapper.Map(providerProfileDto, providerProfile);
            GenericResponse genericResponse = await cabRepository.UpdatePublicProviderInformation(providerProfile, loggedInUserEmail);
            return genericResponse;
        }

       
        #endregion
    }
}
