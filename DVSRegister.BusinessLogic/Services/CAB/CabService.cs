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

        public CabService(ICabRepository cabRepository, IMapper automapper)
        {
            this.cabRepository = cabRepository;
            this.automapper = automapper;  
        }     
        public async Task<List<RoleDto>> GetRoles(decimal tfVersion)
        {
            var list = await cabRepository.GetRoles(tfVersion);
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
        public async Task<List<TrustFrameworkVersionDto>> GetTfVersion()
        {
            var list = await cabRepository.GetTfVersion();
            return automapper.Map<List<TrustFrameworkVersionDto>>(list);
        }
        public bool CheckCompanyInfoEditable(ProviderProfileDto providerProfileDto)
        {
            return providerProfileDto.Services == null || providerProfileDto.Services.Count==0 || // services not added ie certificate info not submitted yet
            providerProfileDto.Services.All(service => service.CertificateReview == null && service.ServiceStatus == ServiceStatusEnum.Submitted) || //certificate info submitted but review not started
            providerProfileDto.Services.All(service => service.CertificateReview == null
            || service.CertificateReview.Where(x=>x.IsLatestReviewVersion).SingleOrDefault()?.CertificateReviewStatus != CertificateReviewEnum.Approved); //none of the services has an Approved status;
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

        public async Task<ProviderProfileDto> GetProviderAndAssignPublishedService(int providerId, int cabId)
        {
            var provider = await cabRepository.GetProvider(providerId, cabId);
            ProviderProfileDto providerDto = automapper.Map<ProviderProfileDto>(provider);
            if(providerDto.Services!=null && providerDto.Services.Count>0)
            {
                var groupedServices = providerDto.Services.GroupBy(s => s.ServiceKey).ToDictionary(g => g.Key, g => g.ToList());
                foreach (var service in providerDto.Services)
                {
                    service.PublishedServiceVersion = groupedServices[service.ServiceKey].FirstOrDefault(s => s.ServiceStatus == ServiceStatusEnum.Published);
                }

            }
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
        public async Task<bool> IsManualServiceLinkedToMultipleServices(int manualServiceId)
        {
            return await cabRepository.IsManualServiceLinkedToMultipleServices(manualServiceId);
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


        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetCompanyValueUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.RegisteredName !=  previousData.RegisteredName)
            {
                previousDataDictionary.Add(Constants.RegisteredName, [previousData.RegisteredName]);
                currentDataDictionary.Add(Constants.RegisteredName, [currentData.RegisteredName]);
            }
            if (currentData.TradingName != previousData.TradingName)
            {
                previousDataDictionary.Add(Constants.TradingName, [string.IsNullOrEmpty(previousData.TradingName) ? Constants.NullFieldsDisplay : previousData.TradingName]);
                currentDataDictionary.Add(Constants.TradingName, [string.IsNullOrEmpty(currentData.TradingName) ? Constants.NullFieldsDisplay : currentData.TradingName]);
            }
          
            if (previousData.HasRegistrationNumber == true && currentData.CompanyRegistrationNumber!= previousData.CompanyRegistrationNumber)
            {               
                currentDataDictionary.Add(Constants.CompanyRegistrationNumber, [currentData.CompanyRegistrationNumber]); 
                previousDataDictionary.Add(Constants.CompanyRegistrationNumber, [previousData.CompanyRegistrationNumber]);
            }
            if (previousData.HasRegistrationNumber == false && currentData.DUNSNumber!= previousData.DUNSNumber)
            {             
              
                currentDataDictionary.Add(Constants.DUNSNumber, [currentData.DUNSNumber]);
                previousDataDictionary.Add(Constants.DUNSNumber, [previousData.DUNSNumber]);
            }
            
            if (previousData.HasParentCompany == true )
            {      
                if (currentData.ParentCompanyRegisteredName != previousData.ParentCompanyRegisteredName)
                {
                    currentDataDictionary.Add(Constants.ParentCompanyRegisteredName, [currentData.ParentCompanyRegisteredName]);
                    previousDataDictionary.Add(Constants.ParentCompanyRegisteredName, [previousData.ParentCompanyRegisteredName]);
                }
                if(currentData.ParentCompanyLocation != previousData.ParentCompanyLocation)
                {
                    currentDataDictionary.Add(Constants.ParenyCompanyLocation, [currentData.ParentCompanyLocation]);
                    previousDataDictionary.Add(Constants.ParenyCompanyLocation, [previousData.ParentCompanyLocation]);
                }                
            }            

            return (currentDataDictionary, previousDataDictionary);
         
        }

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPrimaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.PrimaryContactFullName != previousData.PrimaryContactFullName)
            {
                previousDataDictionary.Add(Constants.PrimaryContactName, [previousData.PrimaryContactFullName]);
                currentDataDictionary.Add(Constants.PrimaryContactName, [currentData.PrimaryContactFullName]);
            }
            if (currentData.PrimaryContactEmail != previousData.PrimaryContactEmail)
            {
                previousDataDictionary.Add(Constants.PrimaryContactEmail, [previousData.PrimaryContactEmail]);
                currentDataDictionary.Add(Constants.PrimaryContactEmail, [currentData.PrimaryContactEmail]);
            }
            if (currentData.PrimaryContactJobTitle != previousData.PrimaryContactJobTitle)
            {
                previousDataDictionary.Add(Constants.PrimaryContactJobTitle, [previousData.PrimaryContactJobTitle]);
                currentDataDictionary.Add(Constants.PrimaryContactJobTitle, [currentData.PrimaryContactJobTitle]);
            }
            if (currentData.PrimaryContactTelephoneNumber != previousData.PrimaryContactTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.PrimaryContactTelephone, [previousData.PrimaryContactTelephoneNumber]);
                currentDataDictionary.Add(Constants.PrimaryContactTelephone, [currentData.PrimaryContactTelephoneNumber]);
            }

            return (currentDataDictionary, previousDataDictionary);

        }

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetSecondaryContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.SecondaryContactFullName != previousData.SecondaryContactFullName)
            {
                previousDataDictionary.Add(Constants.SecondaryContactName, [previousData.SecondaryContactFullName]);
                currentDataDictionary.Add(Constants.SecondaryContactName, [currentData.SecondaryContactFullName]);
            }
            if (currentData.SecondaryContactEmail != previousData.SecondaryContactEmail)
            {
                previousDataDictionary.Add(Constants.SecondaryContactEmail, [previousData.SecondaryContactEmail]);
                currentDataDictionary.Add(Constants.SecondaryContactEmail, [currentData.SecondaryContactEmail]);
            }
            if (currentData.SecondaryContactJobTitle != previousData.SecondaryContactJobTitle)
            {
                previousDataDictionary.Add(Constants.SecondaryContactJobTitle, [previousData.SecondaryContactJobTitle]);
                currentDataDictionary.Add(Constants.SecondaryContactJobTitle, [currentData.SecondaryContactJobTitle]);
            }
            if (currentData.SecondaryContactTelephoneNumber != previousData.SecondaryContactTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.SecondaryContactTelephone, [previousData.SecondaryContactTelephoneNumber]);
                currentDataDictionary.Add(Constants.SecondaryContactTelephone, [currentData.SecondaryContactTelephoneNumber]);
            }

            return (currentDataDictionary, previousDataDictionary);

        }

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetPublicContactUpdates(ProviderProfileDto currentData, ProviderProfileDto previousData)
        {
            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();


            if (currentData.ProviderWebsiteAddress != previousData.ProviderWebsiteAddress)
            {
                previousDataDictionary.Add(Constants.ProviderWebsiteAddress, [previousData.ProviderWebsiteAddress]);
                currentDataDictionary.Add(Constants.ProviderWebsiteAddress, [currentData.ProviderWebsiteAddress]);
            }
            if (currentData.PublicContactEmail != previousData.PublicContactEmail)
            {
                previousDataDictionary.Add(Constants.PublicContactEmail, [string.IsNullOrEmpty(previousData.PublicContactEmail) ? Constants.NullFieldsDisplay : previousData.PublicContactEmail]);
                currentDataDictionary.Add(Constants.PublicContactEmail, [string.IsNullOrEmpty(currentData.PublicContactEmail) ? Constants.NullFieldsDisplay : currentData.PublicContactEmail]);
            }
            if (currentData.ProviderTelephoneNumber != previousData.ProviderTelephoneNumber)
            {
                previousDataDictionary.Add(Constants.ProviderTelephoneNumber, [string.IsNullOrEmpty(previousData.ProviderTelephoneNumber) ? Constants.NullFieldsDisplay : previousData.ProviderTelephoneNumber]);
                currentDataDictionary.Add(Constants.ProviderTelephoneNumber, [string.IsNullOrEmpty(currentData.ProviderTelephoneNumber) ? Constants.NullFieldsDisplay : currentData.ProviderTelephoneNumber]);
            }
            if (currentData.LinkToContactPage != previousData.LinkToContactPage)
            {
                previousDataDictionary.Add(Constants.LinkToContactPage, [string.IsNullOrEmpty(previousData.LinkToContactPage) ? Constants.NullFieldsDisplay : previousData.LinkToContactPage]);
                currentDataDictionary.Add(Constants.LinkToContactPage, [string.IsNullOrEmpty(currentData.LinkToContactPage) ? Constants.NullFieldsDisplay : currentData.LinkToContactPage]);
            }
            

            return (currentDataDictionary, previousDataDictionary);

        }

        public async Task<ProviderProfileDto> GetProviderWithLatestVersionServices(int providerId, int cabId)
        {
            var provider = await cabRepository.GetProviderWithLatestVersionServices(providerId, cabId);
            ProviderProfileDto providerDto = automapper.Map<ProviderProfileDto>(provider);
            return providerDto;
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

        public async Task<GenericResponse> SaveServiceReApplication(ServiceDto serviceDto, string loggedInUserEmail, bool isReupload)
        {
            Service service = new ();
            automapper.Map(serviceDto, service);
            GenericResponse genericResponse = await cabRepository.SaveServiceReApplication(service, loggedInUserEmail, isReupload);
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
