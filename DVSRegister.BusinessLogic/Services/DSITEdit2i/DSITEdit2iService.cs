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
        private readonly Edit2iCheckEmailSender emailSender;        
        private readonly IRemoveProviderService removeProviderService;



        public DSITEdit2iService(IDSITEdit2iRepository dSITEdit2IRepository, IMapper mapper, Edit2iCheckEmailSender emailSender,  IRemoveProviderService removeProviderService)
        {
            this.dSITEdit2IRepository = dSITEdit2IRepository;
            this.mapper = mapper;
            this.emailSender = emailSender;            
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

        public async Task<ServiceDto> GetPreviousServiceDetails(int serviceId)
        {
            Service service = await dSITEdit2IRepository.GetPreviousServiceDetails(serviceId);
            ServiceDto serviceDto = mapper.Map<ServiceDto>(service);
            return serviceDto;

        }

        public async Task<TokenStatusEnum> GetEditServiceTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum tokenStatus = TokenStatusEnum.NA;
            if(tokenDetails.ServiceIds != null && tokenDetails.ServiceIds.Count == 1)
            {
                var service = await dSITEdit2IRepository.GetService(tokenDetails.ServiceIds[0]);
                tokenStatus = service.EditServiceTokenStatus;
            }

            return tokenStatus;
        }

        public async Task<TokenStatusEnum> GetEditProviderTokenStatus(TokenDetails tokenDetails)
        {
            TokenStatusEnum tokenStatus = TokenStatusEnum.NA;
            if (tokenDetails.ProviderProfileId>0 )
            {
                var provider = await dSITEdit2IRepository.GetProvider(tokenDetails.ProviderProfileId);
                tokenStatus = provider.EditProviderTokenStatus;
            }

            return tokenStatus;
        }

        public async Task<(Dictionary<string, List<string>>, Dictionary<string, List<string>>)> GetServiceKeyValue(ServiceDraftDto currentData, ServiceDto previousData)
        {

            var currentDataDictionary = new Dictionary<string, List<string>>();
            var previousDataDictionary = new Dictionary<string, List<string>>();
            GetServiceKeyValueMappings(currentData, previousData, currentDataDictionary, previousDataDictionary);

            if (previousData.TrustFrameworkVersion.Version == Constants.TFVersion0_4)
            {
                GetServiceKeyValueForSchemeMappingsTFV0_4(currentData, previousData, currentDataDictionary, previousDataDictionary);

                await GetServiceKeyValueMappingsForUnderpinningServiceTFV0_4(currentData, previousData, currentDataDictionary, previousDataDictionary);

            }


            return (previousDataDictionary, currentDataDictionary);
        }
      
        private static void GetServiceKeyValueMappings(ServiceDraftDto currentData, ServiceDto previousData, Dictionary<string, List<string>> currentDataDictionary, Dictionary<string, List<string>> previousDataDictionary)
        {
            if (currentData.CompanyAddress != null)
            {
                previousDataDictionary.Add(Constants.RegisteredAddress, [previousData.CompanyAddress]);
                currentDataDictionary.Add(Constants.RegisteredAddress, [currentData.CompanyAddress]);
            }

            if (currentData.ServiceName != null)
            {
                previousDataDictionary.Add(Constants.ServiceName, [previousData.ServiceName]);
                currentDataDictionary.Add(Constants.ServiceName, [currentData.ServiceName]);
            }
            if (currentData.ServiceRoleMappingDraft.Count > 0)
            {
                var roles = previousData.ServiceRoleMapping.Select(item => item.Role.RoleName).ToList();
                previousDataDictionary.Add(Constants.Roles, roles);

                var currentRoles = currentData.ServiceRoleMappingDraft.Select(item => item.Role.RoleName).ToList();
                currentDataDictionary.Add(Constants.Roles, currentRoles);


            }
            var protectionExists = currentData.ServiceQualityLevelMappingDraft.Any(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection);
            var authenticationExists = currentData.ServiceQualityLevelMappingDraft.Any(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication);

            if (protectionExists || currentData.HasGPG44 == false)
            {
                var protectionLevels = previousData.ServiceQualityLevelMapping?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();

                var currentProtectionLevels = currentData.ServiceQualityLevelMappingDraft?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();

                previousDataDictionary.Add(Constants.GPG44Protection, protectionLevels != null && protectionLevels.Count > 0 ? protectionLevels : new List<string> { @Constants.NullFieldsDisplay });
                currentDataDictionary.Add(Constants.GPG44Protection, currentProtectionLevels != null && currentProtectionLevels.Count > 0 ? currentProtectionLevels : new List<string> { @Constants.NullFieldsDisplay });
            }

            if (authenticationExists || currentData.HasGPG44 == false)
            {
                var authenticationLevels = previousData.ServiceQualityLevelMapping?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();

                var currentAuthenticationLevels = currentData.ServiceQualityLevelMappingDraft?
                    .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                    .Select(item => item.QualityLevel.Level)
                    .ToList();

                previousDataDictionary.Add(Constants.GPG44Authentication, authenticationLevels != null && authenticationLevels.Count > 0 ? authenticationLevels : new List<string> { @Constants.NullFieldsDisplay });
                currentDataDictionary.Add(Constants.GPG44Authentication, currentAuthenticationLevels != null && currentAuthenticationLevels.Count > 0 ? currentAuthenticationLevels : new List<string> { @Constants.NullFieldsDisplay });
            }

            #region GPG45 Identity profile
            if (currentData.ServiceIdentityProfileMappingDraft.Count > 0 || currentData.HasGPG45 == false)
            {
                var identityProfiles = previousData.ServiceIdentityProfileMapping?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                var currentIdentityProfiles = currentData.ServiceIdentityProfileMappingDraft?.Select(item => item.IdentityProfile.IdentityProfileName).ToList();
                if (identityProfiles != null && identityProfiles.Count > 0)
                {
                    previousDataDictionary.Add(Constants.GPG45IdentityProfiles, identityProfiles);

                }
                else
                {
                    previousDataDictionary.Add(Constants.GPG45IdentityProfiles, [@Constants.NullFieldsDisplay]);
                }

                if (currentIdentityProfiles != null && currentIdentityProfiles.Count > 0)
                {
                    currentDataDictionary.Add(Constants.GPG45IdentityProfiles, currentIdentityProfiles);
                }
                else
                {
                    currentDataDictionary.Add(Constants.GPG45IdentityProfiles, [@Constants.NullFieldsDisplay]);
                }
            }

            #endregion


            #region Scheme

            if (currentData.ServiceSupSchemeMappingDraft.Count > 0 || currentData.HasSupplementarySchemes == false)
            {
                var supplementarySchemes = previousData.ServiceSupSchemeMapping?.OrderBy(x => x.SupplementarySchemeId).Select(item => item.SupplementaryScheme.SchemeName).ToList();
                var currentSupplementarySchemes = currentData.ServiceSupSchemeMappingDraft?.OrderBy(x => x.SupplementarySchemeId).Select(item => item.SupplementaryScheme.SchemeName).ToList();
                bool areSame = supplementarySchemes != null && currentSupplementarySchemes != null &&
                supplementarySchemes.SequenceEqual(currentSupplementarySchemes);


                if (!areSame)
                {
                    if (supplementarySchemes != null && supplementarySchemes.Count > 0)
                    {
                        previousDataDictionary.Add(Constants.SupplementaryCodes, supplementarySchemes);
                    }
                    else
                    {
                        previousDataDictionary.Add(Constants.SupplementaryCodes, [@Constants.NullFieldsDisplay]);
                    }

                    if (currentSupplementarySchemes != null && currentSupplementarySchemes.Count > 0)
                    {
                        currentDataDictionary.Add(Constants.SupplementaryCodes, currentSupplementarySchemes);
                    }
                    else
                    {
                        currentDataDictionary.Add(Constants.SupplementaryCodes, [@Constants.NullFieldsDisplay]);
                    }
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
        }

        private static void GetServiceKeyValueForSchemeMappingsTFV0_4(ServiceDraftDto currentData, ServiceDto previousData, Dictionary<string, List<string>> currentDataDictionary, Dictionary<string, List<string>> previousDataDictionary)
        {



            if (currentData.ServiceSupSchemeMappingDraft.Count > 0 && currentData.ServiceSupSchemeMappingDraft.Count < previousData?.ServiceSupSchemeMapping?.Count)
            {

                foreach (var schemeMapping in previousData.ServiceSupSchemeMapping)
                {
                    var previousSchemeMapping = previousData.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == schemeMapping.SupplementarySchemeId).FirstOrDefault();
                    if (schemeMapping.SchemeGPG45Mapping != null && schemeMapping.SchemeGPG45Mapping.Count > 0)
                    {

                        var schemeGpg45Mappings = previousSchemeMapping?.SchemeGPG45Mapping?.Select(x => x.IdentityProfile.IdentityProfileName)?.ToList();
                        previousDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, schemeGpg45Mappings ?? [@Constants.NullFieldsDisplay]);

                        var schemeMappingDraft = currentData.ServiceSupSchemeMappingDraft
                        .Where(mapping => mapping.SupplementarySchemeId == schemeMapping.SupplementarySchemeId).FirstOrDefault();

                        var currentSchemeGpg45Mappings = schemeMappingDraft?.SchemeGPG45MappingDraft?.Select(x => x.IdentityProfile.IdentityProfileName).ToList();

                        currentDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, currentSchemeGpg45Mappings ?? [@Constants.NullFieldsDisplay]);


                    }

                    if (schemeMapping?.SchemeGPG44Mapping != null && schemeMapping?.SchemeGPG44Mapping.Count > 0 || schemeMapping?.HasGpg44Mapping == false)
                    {

                        var schemeGpg44ProtectionMappings = previousSchemeMapping?.SchemeGPG44Mapping?.Where(x => x.QualityLevel.QualityType == QualityTypeEnum.Protection)?.Select(x => x.QualityLevel.Level)?.ToList();
                        previousDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, schemeGpg44ProtectionMappings ?? [@Constants.NullFieldsDisplay]);

                        var schemeGpg44AuthenticationMappings = previousSchemeMapping?.SchemeGPG44Mapping?.Where(x => x.QualityLevel.QualityType == QualityTypeEnum.Authentication)?.Select(x => x.QualityLevel.Level)?.ToList();
                        previousDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, schemeGpg44AuthenticationMappings ?? [@Constants.NullFieldsDisplay]);


                        var schemeMappingDraft = currentData.ServiceSupSchemeMappingDraft
                      .Where(mapping => mapping.SupplementarySchemeId == schemeMapping.SupplementarySchemeId).FirstOrDefault();

                        var currentSchemeGpg44Protection = schemeMappingDraft?.SchemeGPG44MappingDraft?
                        .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection).Select(x => x.QualityLevel.Level).ToList();

                        var currentSchemeGpg44Authentication = schemeMappingDraft?.SchemeGPG44MappingDraft?
                        .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication).Select(x => x.QualityLevel.Level).ToList();

                        currentDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, currentSchemeGpg44Protection ?? [@Constants.NullFieldsDisplay]);
                        currentDataDictionary.Add(schemeMapping.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, currentSchemeGpg44Authentication ?? [@Constants.NullFieldsDisplay]);

                    }
                    else
                    {
                        previousDataDictionary.Add(schemeMapping?.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, [@Constants.NullFieldsDisplay]);
                        previousDataDictionary.Add(schemeMapping?.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, [@Constants.NullFieldsDisplay]);
                    }
                }

            }

            else
            {
                foreach (var schemeMappingDraft in currentData.ServiceSupSchemeMappingDraft)
                {

                    if (schemeMappingDraft.SchemeGPG45MappingDraft != null && schemeMappingDraft.SchemeGPG45MappingDraft.Count > 0)
                    {
                        var previousSchemeMappingDraft = previousData.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == schemeMappingDraft.SupplementarySchemeId).FirstOrDefault();

                        if (previousSchemeMappingDraft != null)
                        {
                            var schemeGpg45Mappings = previousSchemeMappingDraft.SchemeGPG45Mapping.Select(x => x.IdentityProfile.IdentityProfileName).ToList();
                            previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, schemeGpg45Mappings);
                        }
                        else
                        {
                            previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, [@Constants.NullFieldsDisplay]);
                        }
                        var currentSchemeGpg45Mappings = schemeMappingDraft?.SchemeGPG45MappingDraft?.Select(x => x.IdentityProfile.IdentityProfileName).ToList();

                        if (currentSchemeGpg45Mappings != null && currentSchemeGpg45Mappings.Count > 0)
                        {
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, currentSchemeGpg45Mappings);
                        }
                        else
                        {
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, [@Constants.NullFieldsDisplay]);
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG45IdentityProfiles, [@Constants.NullFieldsDisplay]);
                        }
                    }


                    if (schemeMappingDraft.SchemeGPG44MappingDraft != null && schemeMappingDraft.SchemeGPG44MappingDraft.Count > 0 || schemeMappingDraft.HasGpg44Mapping == false)
                    {

                        var previousSchemeMappingDraft = previousData.ServiceSupSchemeMapping.Where(x => x.SupplementarySchemeId == schemeMappingDraft.SupplementarySchemeId).FirstOrDefault();
                        if (previousSchemeMappingDraft != null)
                        {
                            var previousSchemeGpg44Protection = previousSchemeMappingDraft.SchemeGPG44Mapping?
                          .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection).Select(x => x.QualityLevel.Level).ToList();

                            var previousSchemeGpg44Authentication = previousSchemeMappingDraft.SchemeGPG44Mapping?
                             .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication).Select(x => x.QualityLevel.Level).ToList();


                            if (previousSchemeGpg44Authentication != null && previousSchemeGpg44Authentication.Count > 0 && previousSchemeGpg44Protection != null && previousSchemeGpg44Protection.Count > 0)
                            {
                                previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, previousSchemeGpg44Protection);
                                previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, previousSchemeGpg44Authentication);

                            }
                            else
                            {
                                previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, [@Constants.NullFieldsDisplay]);
                                previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, [@Constants.NullFieldsDisplay]);

                            }

                        }
                        else
                        {
                            previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, [@Constants.NullFieldsDisplay]);
                            previousDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, [@Constants.NullFieldsDisplay]);

                        }

                        var currentSchemeGpg44Protection = schemeMappingDraft?.SchemeGPG44MappingDraft?
                        .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Protection).Select(x => x.QualityLevel.Level).ToList();

                        var currentSchemeGpg44Authentication = schemeMappingDraft?.SchemeGPG44MappingDraft?
                        .Where(m => m.QualityLevel.QualityType == QualityTypeEnum.Authentication).Select(x => x.QualityLevel.Level).ToList();

                        if (currentSchemeGpg44Protection != null && currentSchemeGpg44Protection.Count > 0 && currentSchemeGpg44Authentication != null
                            && currentSchemeGpg44Authentication.Count > 0)
                        {
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, currentSchemeGpg44Protection);
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, currentSchemeGpg44Authentication);

                        }
                        else
                        {
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Protection, [@Constants.NullFieldsDisplay]);
                            currentDataDictionary.Add(schemeMappingDraft.SupplementaryScheme.SchemeName + ":" + Constants.GPG44Authentication, [@Constants.NullFieldsDisplay]);


                        }
                    }
                }
            }

        }
        private async Task GetServiceKeyValueMappingsForUnderpinningServiceTFV0_4(ServiceDraftDto currentData, ServiceDto previousData, Dictionary<string, List<string>> currentDataDictionary, Dictionary<string, List<string>> previousDataDictionary)
        {
            // previous selected service: published under pinning
            if (previousData.IsUnderPinningServicePublished == true && previousData.UnderPinningServiceId != null)
            {




                if (currentData.IsUnderpinningServicePublished == true && currentData.UnderPinningServiceId != null && currentData.UnderPinningServiceId != previousData.UnderPinningServiceId) //current data is selected from list of manual services 
                {
                    await PopulatePreviousPublishedUnderPinningService(previousDataDictionary, (int)previousData.UnderPinningServiceId);

                    Service currentServiceDto = await dSITEdit2IRepository.GetUnderpinningServiceDetails((int)currentData.UnderPinningServiceId);
                    currentDataDictionary.Add(Constants.UnderpinningServiceName, [currentServiceDto.ServiceName]);
                    currentDataDictionary.Add(Constants.UnderpinningProviderName, [currentServiceDto.Provider.RegisteredName]);
                    currentDataDictionary.Add(Constants.CabOfUnderpinningService, [currentServiceDto.CabUser.Cab.CabName]);
                    currentDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(currentServiceDto.ConformityExpiryDate, "dd MMMM yyyy")]);

                }
                else if (currentData.IsUnderpinningServicePublished == false && currentData.ManualUnderPinningServiceId != null) //current data is selected from list of manual services 
                {
                    await PopulatePreviousPublishedUnderPinningService(previousDataDictionary, (int)previousData.UnderPinningServiceId);
                    ManualUnderPinningService currentSelectedManualService = await dSITEdit2IRepository.GetManualUnderPinningServiceDetails((int)currentData.ManualUnderPinningServiceId);
                    GetKeyValueForSelectedManualUnderpinningService(currentData, currentDataDictionary, currentSelectedManualService);

                }
                else if (currentData.IsUnderpinningServicePublished == false && currentData.ManualUnderPinningServiceId == null && currentData.ManualUnderPinningServiceDraft != null)
                {
                    await PopulatePreviousPublishedUnderPinningService(previousDataDictionary, (int)previousData.UnderPinningServiceId);

                    GetKeyValueForManualUnderpinningDraft(currentData, currentDataDictionary);
                }
            }
            else if (previousData.IsUnderPinningServicePublished == false && previousData.ManualUnderPinningServiceId != null)
            {


                //current is a published service
                if (currentData.IsUnderpinningServicePublished == true && currentData.UnderPinningServiceId != null)
                {
                    await PopulatePreviousManualUnderpinningService(previousDataDictionary, (int)previousData.ManualUnderPinningServiceId);

                    Service currentServiceDto = await dSITEdit2IRepository.GetUnderpinningServiceDetails((int)currentData.UnderPinningServiceId);
                    currentDataDictionary.Add(Constants.UnderpinningServiceName, [currentServiceDto.ServiceName]);
                    currentDataDictionary.Add(Constants.UnderpinningProviderName, [currentServiceDto.Provider.RegisteredName]);
                    currentDataDictionary.Add(Constants.CabOfUnderpinningService, [currentServiceDto.CabUser.Cab.CabName]);
                    currentDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(currentServiceDto.ConformityExpiryDate, "dd MMMM yyyy")]);
                }
                else if (currentData.IsUnderpinningServicePublished == false && currentData.ManualUnderPinningServiceId != null) // manual selected service
                {
                    await PopulatePreviousManualUnderpinningService(previousDataDictionary, (int)previousData.ManualUnderPinningServiceId);

                    ManualUnderPinningService currentSelectedManualService = await dSITEdit2IRepository.GetManualUnderPinningServiceDetails((int)currentData.ManualUnderPinningServiceId);
                    GetKeyValueForSelectedManualUnderpinningService(currentData, currentDataDictionary, currentSelectedManualService);
                }
                else if (currentData.IsUnderpinningServicePublished == false && currentData.ManualUnderPinningServiceId == null && currentData.ManualUnderPinningServiceDraft != null) // manualluy enetered service
                {
                    await PopulatePreviousManualUnderpinningService(previousDataDictionary, (int)previousData.ManualUnderPinningServiceId);
                    GetKeyValueForManualUnderpinningDraft(currentData, currentDataDictionary);
                }


            }
        }

        private async Task PopulatePreviousManualUnderpinningService(Dictionary<string, List<string>> previousDataDictionary, int manualUnderPinningServiceId)
        {
            ManualUnderPinningService previousManualUnderPinningService = await dSITEdit2IRepository.GetManualUnderPinningServiceDetails(manualUnderPinningServiceId);
            // previous underpinning is a manual service
            previousDataDictionary.Add(Constants.UnderpinningServiceName, [previousManualUnderPinningService.ServiceName]);
            previousDataDictionary.Add(Constants.UnderpinningProviderName, [previousManualUnderPinningService.ProviderName]);
            previousDataDictionary.Add(Constants.CabOfUnderpinningService, [previousManualUnderPinningService.Cab.CabName]);
            previousDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(previousManualUnderPinningService.CertificateExpiryDate, "dd MMMM yyyy")]);
        }

        private async Task PopulatePreviousPublishedUnderPinningService(Dictionary<string, List<string>> previousDataDictionary, int underPinningServiceId)
        {
            Service previousServiceDto = await dSITEdit2IRepository.GetUnderpinningServiceDetails(underPinningServiceId);
            previousDataDictionary.Add(Constants.UnderpinningServiceName, [previousServiceDto.ServiceName]);
            previousDataDictionary.Add(Constants.UnderpinningProviderName, [previousServiceDto.Provider.RegisteredName]);
            previousDataDictionary.Add(Constants.CabOfUnderpinningService, [previousServiceDto.CabUser.Cab.CabName]);
            previousDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(previousServiceDto.ConformityExpiryDate, "dd MMMM yyyy")]);
        }

        private static void GetKeyValueForSelectedManualUnderpinningService(ServiceDraftDto currentData, Dictionary<string, List<string>> currentDataDictionary, ManualUnderPinningService currentSelectedManualService)
        {
            if (currentData.ManualUnderPinningServiceDraft.ServiceName != currentSelectedManualService.ServiceName)
            {
                currentDataDictionary.Add(Constants.UnderpinningServiceName, [currentData.ManualUnderPinningServiceDraft.ServiceName]);
            }
            else
            {
                currentDataDictionary.Add(Constants.UnderpinningServiceName, [currentSelectedManualService.ServiceName]);
            }
            if (currentData.ManualUnderPinningServiceDraft.ProviderName != currentSelectedManualService.ProviderName)
            {
                currentDataDictionary.Add(Constants.UnderpinningProviderName, [currentData.ManualUnderPinningServiceDraft.ProviderName]);
            }
            else
            {
                currentDataDictionary.Add(Constants.UnderpinningProviderName, [currentSelectedManualService.ProviderName]);
            }
            if (currentData.ManualUnderPinningServiceDraft.CabId != currentSelectedManualService.CabId)
            {
                currentDataDictionary.Add(Constants.CabOfUnderpinningService, [currentData.ManualUnderPinningServiceDraft.SelectedCabName]);
            }
            else
            {
                currentDataDictionary.Add(Constants.CabOfUnderpinningService, [currentSelectedManualService.Cab.CabName]);
            }
            if (currentData.ManualUnderPinningServiceDraft.CertificateExpiryDate != currentSelectedManualService.CertificateExpiryDate)
            {
                currentDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(currentData.ManualUnderPinningServiceDraft.CertificateExpiryDate, "dd MMMM yyyy")]);
            }
            else
            {
                currentDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(currentSelectedManualService.CertificateExpiryDate, "dd MMMM yyyy")]);
            }
        }

        private static void GetKeyValueForManualUnderpinningDraft(ServiceDraftDto currentData, Dictionary<string, List<string>> currentDataDictionary)
        {

            // new manually entered service
            currentDataDictionary.Add(Constants.UnderpinningServiceName, [currentData.ManualUnderPinningServiceDraft.ServiceName]);
            currentDataDictionary.Add(Constants.UnderpinningProviderName, [currentData.ManualUnderPinningServiceDraft.ProviderName]);
            currentDataDictionary.Add(Constants.CabOfUnderpinningService, [currentData.ManualUnderPinningServiceDraft.SelectedCabName]);
            currentDataDictionary.Add(Constants.UnderpiningExpiryDate, [Helper.GetLocalDateTime(currentData.ManualUnderPinningServiceDraft.CertificateExpiryDate, "dd MMMM yyyy")]);
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
                    var (previous, current) = await GetServiceKeyValue(serviceDraft, serviceDraft.Service);
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
