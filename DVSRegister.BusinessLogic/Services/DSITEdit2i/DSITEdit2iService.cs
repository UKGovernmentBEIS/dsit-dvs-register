using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;

namespace DVSRegister.BusinessLogic.Services
{
    public class DSITEdit2iService : IDSITEdit2iService
    {
        private readonly IDSITEdit2iRepository dSITEdit2IRepository;
        private readonly IMapper mapper;
        private readonly IEmailSender emailSender;
        private readonly IRemoveProvider2iRepository removeProvider2IRepository;


        public DSITEdit2iService(IDSITEdit2iRepository dSITEdit2IRepository, IMapper mapper, IEmailSender emailSender, IRemoveProvider2iRepository removeProvider2IRepository)
        {
            this.dSITEdit2IRepository = dSITEdit2IRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.removeProvider2IRepository = removeProvider2IRepository;
        }
        public async Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId)
        {
            ProviderDraftToken providerDraftToken = await dSITEdit2IRepository.GetProviderDraftToken(token, tokenId);
            ProviderDraftTokenDto providerDraftTokenDto = mapper.Map<ProviderDraftTokenDto>(providerDraftToken);
            return providerDraftTokenDto;

        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateProviderAndServiceStatusAndData(providerProfileId, providerDraftId);       
            return genericResponse;
        }

        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveProviderDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelProviderUpdates(int providerProfileId, int providerDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelProviderUpdates(providerProfileId, providerDraftId);
            return genericResponse;
        }


        public async Task<ServiceDraftTokenDto> GetServiceChangesByToken(string token, string tokenId)
        {
            ServiceDraftToken serviceDraftToken = await dSITEdit2IRepository.GetServiceDraftToken(token, tokenId);
            ServiceDraftTokenDto serviceDraftTokenDto = mapper.Map<ServiceDraftTokenDto>(serviceDraftToken);
            return serviceDraftTokenDto;

        }

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetServiceKeyValue(ServiceDraftDto currentData, ServiceDto previousData)
        {

            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();

            if (currentData.CompanyAddress != null)
            {
                previousDataDictionary.Add("Registered address", [previousData.CompanyAddress]);
                currentDataDictionary.Add("Registered address", [currentData.CompanyAddress]);
            }

            if (currentData.ServiceName != null)
            {
                previousDataDictionary.Add("Service name", [previousData.ServiceName]);
                currentDataDictionary.Add("Service name", [currentData.ServiceName]);
            }



            if (currentData.ServiceRoleMappingDraft.Count > 0)
            {
                var roles = previousData.ServiceRoleMapping.Select(item => item.Role.RoleName).ToList();
                previousDataDictionary.Add("Roles certified against", roles);

                var currentRoles = currentData.ServiceRoleMappingDraft.Select(item => item.Role.RoleName).ToList();
                currentDataDictionary.Add("Roles certified against", currentRoles);


            }


            if (currentData.ServiceQualityLevelMappingDraft.Count > 0 || currentData.HasGPG44 == false)
            {
                var protectionLevels = previousData.ServiceQualityLevelMapping?
                   .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                   .Select(item => item.QualityLevel.Level)
                   .ToList();

                var currentProtectionLevels = currentData.ServiceQualityLevelMappingDraft?
                  .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                  .Select(item => item.QualityLevel.Level)
                  .ToList();


                if (protectionLevels != null && protectionLevels.Count > 0)
                {
                    previousDataDictionary.Add("GPG44 level of protection", protectionLevels);

                }
                else
                {
                    previousDataDictionary.Add("GPG44 level of protection", ["Not certified against GPG44"]);
                }

                if (currentProtectionLevels != null && currentProtectionLevels.Count > 0)
                {
                    currentDataDictionary.Add("GPG44 level of protection", currentProtectionLevels);
                }
                else
                {
                    currentDataDictionary.Add("GPG44 level of protection", ["Not certified against GPG44"]);
                }

                var authenticationLevels = previousData.ServiceQualityLevelMapping?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();

                var currentAuthenticationLevels = currentData.ServiceQualityLevelMappingDraft?
                   .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                   .Select(item => item.QualityLevel.Level)
                   .ToList();

                if (authenticationLevels != null && authenticationLevels.Count > 0)
                {
                    previousDataDictionary.Add("GPG44 quality of authentication", authenticationLevels);

                }
                else
                {
                    previousDataDictionary.Add("GPG44 quality of authentication", ["Not certified against GPG44"]);
                }

                if (currentAuthenticationLevels != null && currentAuthenticationLevels.Count > 0)
                {
                    currentDataDictionary.Add("GPG44 quality of authentication", currentAuthenticationLevels);
                }
                else
                {
                    currentDataDictionary.Add("GPG44 level of authentication", ["Not certified against GPG44"]);
                }
            }



            #region GPG45 Identity profile
            if (currentData.ServiceIdentityProfileMappingDraft.Count > 0 || currentData.HasGPG45 == false)
            {
                var identityProfiles = previousData.ServiceIdentityProfileMapping?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                var currentIdentityProfiles = currentData.ServiceIdentityProfileMappingDraft?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                if (identityProfiles != null && identityProfiles.Count > 0)
                {
                    previousDataDictionary.Add("GPG45 identity profiles", identityProfiles);

                }
                else
                {
                    previousDataDictionary.Add("GPG45 identity profiles", ["Not certified against any identity profiles"]);
                }

                if (currentIdentityProfiles != null && currentIdentityProfiles.Count > 0)
                {
                    currentDataDictionary.Add("GPG45 identity profiles", currentIdentityProfiles);
                }
                else
                {
                    currentDataDictionary.Add("GPG45 identity profiles", ["Not certified against any identity profiles"]);
                }
            }

            #endregion


            #region Scheme

            if (currentData.ServiceSupSchemeMappingDraft.Count > 0 || currentData.HasSupplementarySchemes == false)
            {
                var supplementarySchemes = previousData.ServiceSupSchemeMapping?.Select(item => item.SupplementaryScheme.SchemeName).ToList();
                var currentSupplementarySchemes = currentData.ServiceSupSchemeMappingDraft?.Select(item => item.SupplementaryScheme.SchemeName).ToList();
                if (supplementarySchemes != null && supplementarySchemes.Count > 0)
                {
                    previousDataDictionary.Add("Supplementary Codes", supplementarySchemes);
                }
                else
                {
                    previousDataDictionary.Add("Supplementary Codes", ["Not certified against any supplementary schemes"]);
                }

                if (currentSupplementarySchemes != null && currentSupplementarySchemes.Count > 0)
                {
                    currentDataDictionary.Add("Supplementary Codes", currentSupplementarySchemes);
                }
                else
                {
                    currentDataDictionary.Add("Supplementary Codes", ["Not certified against any supplementary schemes"]);
                }
            }

            #endregion


            if (currentData.ConformityIssueDate != null)
            {
                previousDataDictionary.Add("Issue date", [Helper.GetLocalDateTime(previousData.ConformityIssueDate, "dd MMMM yyyy")]);
                currentDataDictionary.Add("Issue date", [Helper.GetLocalDateTime(currentData.ConformityIssueDate, "dd MMMM yyyy")]);
            }

            if (currentData.ConformityExpiryDate != null)
            {
                previousDataDictionary.Add("Expiry date", [Helper.GetLocalDateTime(previousData.ConformityExpiryDate, "dd MMMM yyyy")]);
                currentDataDictionary.Add("Expiry date", [Helper.GetLocalDateTime(previousData.ConformityExpiryDate, "dd MMMM yyyy")]);
            }

            return (previousDataDictionary, currentDataDictionary);
        }


        public async Task<GenericResponse> UpdateServiceStatusAndData(int serviceId, int serviceDraftId)
        {
            // update provider status
            
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateServiceStatusAndData(serviceId, serviceDraftId);


            if (genericResponse.Success)
            {
                ProviderProfile providerProfile = await removeProvider2IRepository.GetProviderWithAllServices(serviceId);
                ProviderStatusEnum providerStatus = ServiceHelper.GetProviderStatus(providerProfile.Services, providerProfile.ProviderStatus);
                genericResponse = await removeProvider2IRepository.UpdateProviderStatus(providerProfile.Id, providerStatus, "DSIT", EventTypeEnum.ServiceEdit2i);


            }
            return genericResponse;
        }

        public async Task<bool> RemoveServiceDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveServiceDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelServiceUpdates(int serviceId, int serviceDraftId)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelServiceUpdates(serviceId, serviceDraftId);
            return genericResponse;
        }
    }
}
