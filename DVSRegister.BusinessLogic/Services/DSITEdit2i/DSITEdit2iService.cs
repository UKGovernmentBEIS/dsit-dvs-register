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
        private readonly IRemoveProviderRepository removeProviderRepository;
        private readonly IRemoveProviderService removeProviderService;



        public DSITEdit2iService(IDSITEdit2iRepository dSITEdit2IRepository, IMapper mapper, IEmailSender emailSender, IRemoveProviderRepository removeProviderRepository, IRemoveProviderService removeProviderService)
        {
            this.dSITEdit2IRepository = dSITEdit2IRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;
            this.removeProviderRepository = removeProviderRepository;
            this.removeProviderService = removeProviderService;
        }
        public async Task<ProviderDraftTokenDto> GetProviderChangesByToken(string token, string tokenId)
        {
            ProviderDraftToken providerDraftToken = await dSITEdit2IRepository.GetProviderDraftToken(token, tokenId);
            ProviderDraftTokenDto providerDraftTokenDto = mapper.Map<ProviderDraftTokenDto>(providerDraftToken);
            return providerDraftTokenDto;

        }

        public async Task<GenericResponse> UpdateProviderAndServiceStatusAndData(ProviderProfileDraftDto providerProfileDraft)
        {
            
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateProviderAndServiceStatusAndData(providerProfileDraft.ProviderProfileId, providerProfileDraft.Id);    
            if(genericResponse.Success) 
            {

                var (previous, current) = GetProviderKeyValue(providerProfileDraft, providerProfileDraft.Provider);
                string currentData = Helper.ConcatenateKeyValuePairs(current);
                string previousData = Helper.ConcatenateKeyValuePairs(previous);

               string userEmail = providerProfileDraft.User.Email;
               await emailSender.EditProviderAccepted(userEmail, userEmail, providerProfileDraft.Provider.RegisteredName, currentData, previousData);
            }
            return genericResponse;
        }

        public async Task<bool> RemoveProviderDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveProviderDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelProviderUpdates(ProviderProfileDraftDto providerProfileDraft)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelProviderUpdates(providerProfileDraft.ProviderProfileId, providerProfileDraft.Id);

            if(genericResponse.Success)
            {
                string userEmail = providerProfileDraft.User.Email;
                await emailSender.EditProviderDeclined(userEmail, userEmail, providerProfileDraft.Provider.RegisteredName);
            }
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
                    previousDataDictionary.Add("GPG44 level of protection", [@Constants.NullFieldsDisplay]);
                }

                if (currentProtectionLevels != null && currentProtectionLevels.Count > 0)
                {
                    currentDataDictionary.Add("GPG44 level of protection", currentProtectionLevels);
                }
                else
                {
                    currentDataDictionary.Add("GPG44 level of protection", [@Constants.NullFieldsDisplay]);
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
                    previousDataDictionary.Add("GPG44 quality of authentication", [@Constants.NullFieldsDisplay]);
                }

                if (currentAuthenticationLevels != null && currentAuthenticationLevels.Count > 0)
                {
                    currentDataDictionary.Add("GPG44 quality of authentication", currentAuthenticationLevels);
                }
                else
                {
                    currentDataDictionary.Add("GPG44 level of authentication", [@Constants.NullFieldsDisplay]);
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
                    previousDataDictionary.Add("GPG45 identity profiles", [@Constants.NullFieldsDisplay]);
                }

                if (currentIdentityProfiles != null && currentIdentityProfiles.Count > 0)
                {
                    currentDataDictionary.Add("GPG45 identity profiles", currentIdentityProfiles);
                }
                else
                {
                    currentDataDictionary.Add("GPG45 identity profiles", [@Constants.NullFieldsDisplay]);
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
                    previousDataDictionary.Add("Supplementary Codes", [@Constants.NullFieldsDisplay]);
                }

                if (currentSupplementarySchemes != null && currentSupplementarySchemes.Count > 0)
                {
                    currentDataDictionary.Add("Supplementary Codes", currentSupplementarySchemes);
                }
                else
                {
                    currentDataDictionary.Add("Supplementary Codes", [@Constants.NullFieldsDisplay]);
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
                currentDataDictionary.Add("Expiry date", [Helper.GetLocalDateTime(currentData.ConformityExpiryDate, "dd MMMM yyyy")]);
            }

            return (previousDataDictionary, currentDataDictionary);
        }

        public (Dictionary<string, List<string>>, Dictionary<string, List<string>>) GetProviderKeyValue(ProviderProfileDraftDto currentData, ProviderProfileDto previousData)
        {
            var previousDataDictionary = new Dictionary<string, List<string>>();

            var currentDataDictionary = new Dictionary<string, List<string>>();

            if (currentData.RegisteredName != null)
            {
                previousDataDictionary.Add("Registered name", [previousData.RegisteredName]);
                currentDataDictionary.Add("Registered name", [currentData.RegisteredName]);
            }

            if (currentData.TradingName != null)
            {
                previousDataDictionary.Add("Trading name", [previousData.TradingName]);
                currentDataDictionary.Add("Trading name", [currentData.TradingName]);
            }

            if (currentData.PrimaryContactFullName != null)
            {
                previousDataDictionary.Add("Primary contact full name", [previousData.PrimaryContactFullName]);
                currentDataDictionary.Add("Primary contact full name", [currentData.PrimaryContactFullName]);
            }
            if (currentData.PrimaryContactEmail != null)
            {
                previousDataDictionary.Add("Primary contact email", [previousData.PrimaryContactEmail]);
                currentDataDictionary.Add("Primary contact email", [currentData.PrimaryContactEmail]);
            }
            if (currentData.PrimaryContactJobTitle != null)
            {
                previousDataDictionary.Add("Primary contact job title", [previousData.PrimaryContactJobTitle]);
                currentDataDictionary.Add("primary contact job title", [currentData.PrimaryContactJobTitle]);
            }
            if (currentData.PrimaryContactTelephoneNumber != null)
            {
                previousDataDictionary.Add("Primary contact telephone number", [previousData.PrimaryContactTelephoneNumber]);
                currentDataDictionary.Add("Primary contact telephone number", [currentData.PrimaryContactTelephoneNumber]);
            }

            if (currentData.SecondaryContactFullName != null)
            {
                previousDataDictionary.Add("Secondary contact full name", [previousData.SecondaryContactFullName]);
                currentDataDictionary.Add("Secondary contact full name", [currentData.SecondaryContactFullName]);
            }
            if (currentData.SecondaryContactEmail != null)
            {
                previousDataDictionary.Add("Secondary contact email", [previousData.SecondaryContactEmail]);
                currentDataDictionary.Add("Secondary contact email", [currentData.SecondaryContactEmail]);
            }
            if (currentData.SecondaryContactJobTitle != null)
            {
                previousDataDictionary.Add("Secondary contact job title", [previousData.SecondaryContactJobTitle]);
                currentDataDictionary.Add("Secondary contact job title", [currentData.SecondaryContactJobTitle]);
            }
            if (currentData.SecondaryContactTelephoneNumber != null)
            {
                previousDataDictionary.Add("Secondary contact telephone number", [previousData.SecondaryContactTelephoneNumber]);
                currentDataDictionary.Add("Secondary contact telephone number", [currentData.SecondaryContactTelephoneNumber]);
            }

            if (currentData.ProviderWebsiteAddress != null)
            {
                previousDataDictionary.Add("Provider website address", [previousData.ProviderWebsiteAddress]);
                currentDataDictionary.Add("Provider website address", [currentData.ProviderWebsiteAddress]);
            }

            if (currentData.PublicContactEmail != null)
            {
                previousDataDictionary.Add("Public contact email", [previousData.PublicContactEmail]);
                currentDataDictionary.Add("Public contact email", [currentData.PublicContactEmail]);
            }

            if (currentData.ProviderTelephoneNumber != null)
            {
                previousDataDictionary.Add("Provider telephone number", [previousData.ProviderTelephoneNumber]);
                currentDataDictionary.Add("Provider telephone number", [currentData.ProviderTelephoneNumber]);
            }

            return (previousDataDictionary, currentDataDictionary);
        }


        public async Task<GenericResponse> UpdateServiceStatusAndData(ServiceDraftDto serviceDraft)
        {
            // update service data            
            GenericResponse genericResponse = await dSITEdit2IRepository.UpdateServiceStatusAndData(serviceDraft.ServiceId, serviceDraft.Id);
            if (genericResponse.Success)
            {           
               //update provider status by priority
                genericResponse = await removeProviderService.UpdateProviderStatus(serviceDraft.ProviderProfileId, "DSIT", EventTypeEnum.ServiceEdit2i, TeamEnum.DSIT);

                if(genericResponse.Success) 
                {
                    var (previous, current) = GetServiceKeyValue(serviceDraft, serviceDraft.Service);
                    string currentData = Helper.ConcatenateKeyValuePairs(current);
                    string previousData = Helper.ConcatenateKeyValuePairs(previous);

                    string userEmail = serviceDraft.User.Email;
                    await emailSender.EditServiceAccepted(userEmail, userEmail, serviceDraft.Provider.RegisteredName, serviceDraft.Service.ServiceName, currentData, previousData);
                }


            }
            return genericResponse;
        }

        public async Task<bool> RemoveServiceDraftToken(string token, string tokenId)
        {
            return await dSITEdit2IRepository.RemoveServiceDraftToken(token, tokenId);
        }

        public async Task<GenericResponse> CancelServiceUpdates(ServiceDraftDto serviceDraft)
        {
            GenericResponse genericResponse = await dSITEdit2IRepository.CancelServiceUpdates(serviceDraft.ServiceId, serviceDraft.Id);
            if(genericResponse.Success) 
            {

                //update provider status by priority
               
                genericResponse = await removeProviderService.UpdateProviderStatus(serviceDraft.ProviderProfileId, "DSIT", EventTypeEnum.ServiceEdit2i, TeamEnum.DSIT);
                if (genericResponse.Success)
                {
                    string userEmail = serviceDraft.User.Email;
                    await emailSender.EditServiceDeclined(userEmail, userEmail, serviceDraft.Provider.RegisteredName, serviceDraft.Service.ServiceName);
                }
           
            }
            return genericResponse;
        }
    }
}
