﻿using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
namespace DVSRegister.BusinessLogic.Services.CAB
{
    public interface ICabService
    {
        public Task<List<RoleDto>> GetRoles();
        public Task<List<IdentityProfileDto>> GetIdentityProfiles();
        public Task<List<SupplementarySchemeDto>> GetSupplementarySchemes();
        public Task<bool> CheckProviderRegisteredNameExists(string registeredName, int providerId = 0);       
        public Task<List<ProviderProfileDto>> GetProviders(int cabId, string searchText = "");
        public Task<ProviderProfileDto> GetProvider(int providerId, int cabId);
        public Task<ServiceDto> GetServiceDetails(int serviceId, int cabId);
        public Task<List<ServiceDto>> GetServiceList(int serviceId, int cabId);
        public Task<List<QualityLevelDto>> GetQualitylevels();
        public Task<bool> CheckValidCabAndProviderProfile(int providerId, int cabId);
        public bool CheckCompanyInfoEditable(ProviderProfileDto providerProfileDto);
        public Task<ServiceDto> GetServiceDetailsWithProvider(int serviceId, int cabId);
        public Task<(bool, List<CabTransferRequestDto>)> GetPendingReassignRequests(int cabId);
        public Task<List<string>> GetCabEmailListForProvider(int providerId);

        #region Save/update
        public Task<GenericResponse> SaveProviderProfile(ProviderProfileDto providerProfile, string loggedInUserEmail);
        public Task<GenericResponse> SaveService(ServiceDto serviceDto, string loggedInUserEmail);
        public Task<GenericResponse> UpdateCompanyInfo(ProviderProfileDto providerProfileDto, string loggedInUserEmail);
        public Task<GenericResponse> UpdatePrimaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail);
        public Task<GenericResponse> UpdateSecondaryContact(ProviderProfileDto providerProfileDto, string loggedInUserEmail);
        public Task<GenericResponse> UpdatePublicProviderInformation(ProviderProfileDto providerProfileDto, string loggedInUserEmail);
        public Task<GenericResponse> SaveServiceReApplication(ServiceDto serviceDto, string loggedInUserEmail);
        public Task<GenericResponse> SaveServiceAmendments(ServiceDto serviceDto, string existingFileLink, int existingServiceCabId, int cabId, string loggedInUserEmail);      
        public bool CanDeleteCertificate(string currentFileLink, string existingFileLink, int existingServiceCabId, int cabId);

        #endregion
    }
}
