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

        public Dictionary<string, List<string>> GetPreviousDataKeyPair(ServiceDraftDto currentData, ServiceDto previousData )
        {
            var summaryData = new Dictionary<string, List<string>>();

            if (currentData.CompanyAddress != null)
            {
                summaryData.Add("Registered Address",  [previousData.CompanyAddress]);
            }

            if (currentData.ServiceName != null)
            {
                summaryData.Add("Service Name", [previousData.ServiceName]);
            }

           

            if (currentData.ServiceRoleMappingDraft.Count > 0)
            {
                var roles = previousData.ServiceRoleMapping.Select(item => item.Role.RoleName).ToList();
                summaryData.Add("Roles certified against", roles);
            }


            if (currentData.ServiceQualityLevelMappingDraft.Count > 0 || (previousData.HasGPG44 == true && currentData.HasGPG44 == false))
            {
                var protectionLevels = previousData.ServiceQualityLevelMapping?
                   .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                   .Select(item => item.QualityLevel.Level)
                   .ToList();
                if (protectionLevels != null && protectionLevels.Count > 0)
                {
                    summaryData.Add("GPG44 level of protection", protectionLevels);
                }
                else
                {
                    summaryData.Add("GPG44 level of protection", ["Not certified against GPG44"]);
                }                             

                var authenticationLevels = previousData.ServiceQualityLevelMapping?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();
                if(authenticationLevels!= null && authenticationLevels.Count > 0)
                {
                    summaryData.Add("GPG44 quality of authentication", authenticationLevels);
                }
                else
                {
                    summaryData.Add("GPG44 quality of authentication", ["Not certified against GPG44"]);
                }
            }
            else if (previousData.HasGPG44 == false )
            {
                summaryData.Add("GPG44 level of protection", ["Not certified against GPG44"]); 

            }


            #region GPG$% Identity profile
            if (currentData.ServiceIdentityProfileMappingDraft.Count > 0 || (previousData.HasGPG45 == true && currentData.HasGPG45 == false))
            {
                var identityProfiles = previousData.ServiceIdentityProfileMapping?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                if (identityProfiles != null && identityProfiles.Count > 0)
                {
                    summaryData.Add("GPG45 identity profiles", identityProfiles);
                }
                else
                {
                    summaryData.Add("GPG45 identity profiles", ["Not certified against any identity profiles"]);
                }
            }
            else if (previousData.HasGPG45 == false)
            {
                summaryData.Add("GPG45 identity profiles", ["Not certified against any identity profiles"]);

            }
            #endregion


            #region Scheme

            if (currentData.ServiceSupSchemeMappingDraft.Count > 0 || (previousData.HasSupplementarySchemes == true && currentData.HasSupplementarySchemes == false))
            {
                var supplementarySchemes = previousData.ServiceSupSchemeMapping?.Select(item => item.SupplementaryScheme.SchemeName).ToList();
                if (supplementarySchemes != null && supplementarySchemes.Count > 0)
                {
                    summaryData.Add("Supplementary Codes", supplementarySchemes);
                }
                else
                {
                    summaryData.Add("Supplementary Codes", ["Not certified against any supplementary schemes"]);
                }
            }
            else if (previousData.HasSupplementarySchemes == false)
            {
                summaryData.Add("Supplementary Codes", ["Not certified against any supplementary schemes"]);

            }
            #endregion


            if (currentData.ConformityIssueDate != null)
            {
                summaryData.Add("Issue date", [Helper.GetLocalDateTime(previousData.ConformityIssueDate, "dd MMMM yyyy")]);
            }

            if (currentData.ConformityExpiryDate != null)
            {
                summaryData.Add("Expiry date", [Helper.GetLocalDateTime(previousData.ConformityExpiryDate, "dd MMMM yyyy")]);
            }
            return summaryData;

        }


        public Dictionary<string, List<string>> GetCurrentDataKeyPair(ServiceDraftDto currentData)
        {
            var summaryData = new Dictionary<string, List<string>>();

            if (currentData.CompanyAddress != null)
            {
                summaryData.Add("Registered address", [currentData.CompanyAddress]);
            }

            if (currentData.ServiceName != null)
            {
                summaryData.Add("Service name", [currentData.ServiceName]);
            }

            if (currentData.ServiceRoleMappingDraft.Count > 0)
            {
                var roles = currentData.ServiceRoleMappingDraft.Select(item => item.Role.RoleName).ToList();
                summaryData.Add("Roles certified against", roles);
            }

            #region GPG44

            if (currentData.ServiceQualityLevelMappingDraft.Count > 0)
            {
                var protectionLevels = currentData.ServiceQualityLevelMappingDraft?
                   .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                   .Select(item => item.QualityLevel.Level)
                   .ToList();
                summaryData.Add("GPG44 level of protection", protectionLevels);

                var authenticationLevels = currentData.ServiceQualityLevelMappingDraft?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();
                summaryData.Add("GPG44 quality of authentication", authenticationLevels);
            }
            else if (currentData.HasGPG44 != null && currentData.HasGPG44 == false)
            {
                summaryData.Add("GPG44 level of protection", ["Not certified against GPG44"]);

            }
            #endregion

            #region Identity profile

            if (currentData.ServiceIdentityProfileMappingDraft.Count > 0)
            {
                var identityProfiles = currentData.ServiceIdentityProfileMappingDraft?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                if (identityProfiles != null && identityProfiles.Count > 0)
                {
                    summaryData.Add("GPG45 identity profiles", identityProfiles);
                }
            }
            else if (currentData.HasGPG45 != null && currentData.HasGPG45 == false)
            {
                summaryData.Add("GPG45 identity profiles", ["Not certified against any identity profiles"]);

            }

            #endregion


            #region Supplementary scheme
            if (currentData.ServiceSupSchemeMappingDraft.Count > 0)
            {
                var supplementarySchemes = currentData.ServiceSupSchemeMappingDraft?.Select(item => item.SupplementaryScheme.SchemeName).ToList();
                if (supplementarySchemes != null && supplementarySchemes.Count > 0)
                {
                    summaryData.Add("Supplementary Codes", supplementarySchemes);
                }
            }
            else if (currentData.HasSupplementarySchemes != null && currentData.HasSupplementarySchemes == false)
            {
                summaryData.Add("Supplementary Codes", ["Not certified against any supplementary schemes"]);

            }
            #endregion

            if (currentData.ConformityIssueDate != null)
            {
                summaryData.Add("Issue date", [Helper.GetLocalDateTime(currentData.ConformityIssueDate, "dd MMMM yyyy")]);
            }

            if (currentData.ConformityExpiryDate != null)
            {
                summaryData.Add("Expiry date", [Helper.GetLocalDateTime(currentData.ConformityExpiryDate, "dd MMMM yyyy")]);
            }

            return summaryData;

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
