using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.CommonUtility;

namespace DVSRegister.Models
{
    public static class ViewModelHelper
    {

        public static DateViewModel GetDayMonthYear(DateTime? dateTime)
        {
            DateViewModel dateViewModel = new();
            DateTime conformityIssueDate = Convert.ToDateTime(dateTime);
            dateViewModel.Day = conformityIssueDate.Day;
            dateViewModel.Month = conformityIssueDate.Month;
            dateViewModel.Year = conformityIssueDate.Year;
            return dateViewModel;
        }

        #region Clear selected data in session if selection changed from yes to no
        public static void ClearGpg44(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.QualityLevelViewModel != null)
            {
                summaryViewModel.QualityLevelViewModel.SelectedQualityofAuthenticators = [];
                summaryViewModel.QualityLevelViewModel.SelectedLevelOfProtections = [];
            }
            if (summaryViewModel.SchemeQualityLevelMapping != null)
            {
                summaryViewModel.SchemeQualityLevelMapping.ForEach(item => item.HasGPG44 = false);
                summaryViewModel.SchemeQualityLevelMapping.ForEach(item => item.QualityLevel.SelectedLevelOfProtections = []);
                summaryViewModel.SchemeQualityLevelMapping.ForEach(item => item.QualityLevel.SelectedQualityofAuthenticators = []);
            }

        }
        public static void ClearGpg45(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.IdentityProfileViewModel != null)
                summaryViewModel.IdentityProfileViewModel.SelectedIdentityProfiles = [];          
        }

        public static void ClearSchemes(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.SupplementarySchemeViewModel != null)
                summaryViewModel.SupplementarySchemeViewModel.SelectedSupplementarySchemes = [];
            if (summaryViewModel.SchemeIdentityProfileMapping != null)
                summaryViewModel.SchemeIdentityProfileMapping = [];
            if (summaryViewModel.SchemeQualityLevelMapping != null)
                summaryViewModel.SchemeQualityLevelMapping = [];
        }

        public static void ClearSchemeGpg44(ServiceSummaryViewModel summaryViewModel, int shcemeId)
        {
            if (summaryViewModel.SchemeQualityLevelMapping != null && summaryViewModel.SchemeQualityLevelMapping.Count>0)
            {
                var schemeQualityLevelMapping = summaryViewModel.SchemeQualityLevelMapping.Where(x => x.SchemeId == shcemeId).FirstOrDefault();
                if(schemeQualityLevelMapping!=null && schemeQualityLevelMapping.HasGPG44 == false)
                {
                    if(schemeQualityLevelMapping.QualityLevel!=null)
                    {
                        schemeQualityLevelMapping.QualityLevel.SelectedQualityofAuthenticators = [];
                        schemeQualityLevelMapping.QualityLevel.SelectedLevelOfProtections = [];
                    }
                   
                }
            }

        }

        public static void ClearTFVersion0_4Fields(ServiceSummaryViewModel summaryViewModel)
        {
            summaryViewModel.ServiceType = null;
            summaryViewModel.SelectedManualUnderPinningServiceId = null;
            summaryViewModel.SelectedUnderPinningServiceId = null;
            summaryViewModel.IsUnderpinningServicePublished =null;
            summaryViewModel.SchemeQualityLevelMapping = [];
            summaryViewModel.SchemeIdentityProfileMapping = [];
            summaryViewModel.UnderPinningServiceName=null;
            summaryViewModel.UnderPinningProviderName=null;
            summaryViewModel.SelectCabViewModel=null;
            summaryViewModel.UnderPinningServiceExpiryDate = null;


        }
        public static void ClearUnderPinningServiceFields(ServiceSummaryViewModel summaryViewModel)
        {
            if(summaryViewModel.ServiceType == CommonUtility.Models.Enums.ServiceTypeEnum.UnderPinning 
                || summaryViewModel.ServiceType == CommonUtility.Models.Enums.ServiceTypeEnum.Neither)
            {
                summaryViewModel.SelectedManualUnderPinningServiceId = null;
                summaryViewModel.SelectedUnderPinningServiceId = null;
                summaryViewModel.IsUnderpinningServicePublished = null;            
                summaryViewModel.UnderPinningServiceName = null;
                summaryViewModel.UnderPinningProviderName = null;
                summaryViewModel.SelectCabViewModel = null;
                summaryViewModel.UnderPinningServiceExpiryDate = null;
            }
        }

        public static void ClearUnderPinningServiceFieldsBeforeManualEntry(ServiceSummaryViewModel summaryViewModel)
        {
            if (summaryViewModel.ServiceType == CommonUtility.Models.Enums.ServiceTypeEnum.WhiteLabelled)
            {
                summaryViewModel.SelectedManualUnderPinningServiceId = null;
                summaryViewModel.SelectedUnderPinningServiceId = null;      
                summaryViewModel.UnderPinningServiceName = null;
                summaryViewModel.UnderPinningProviderName = null;
                summaryViewModel.SelectCabViewModel = null;
                summaryViewModel.UnderPinningServiceExpiryDate = null;
            }
        }

        public static void MapTFVersion0_4Fields(ServiceSummaryViewModel summaryViewModel, ServiceDto serviceDto)
        {
            if (summaryViewModel?.TFVersionViewModel?.SelectedTFVersion?.Version == Constants.TFVersion0_4)
            {
                serviceDto.ServiceType = summaryViewModel?.ServiceType;

                if (serviceDto.ServiceType == ServiceTypeEnum.WhiteLabelled)
                {
                    if (summaryViewModel?.SelectedUnderPinningServiceId != null && summaryViewModel?.SelectedUnderPinningServiceId > 0 && summaryViewModel.IsUnderpinningServicePublished == true)
                        serviceDto.UnderPinningServiceId = summaryViewModel?.SelectedUnderPinningServiceId;
                    else
                    {
                        if (summaryViewModel?.SelectedManualUnderPinningServiceId != null && summaryViewModel?.SelectedManualUnderPinningServiceId > 0)
                        {
                            serviceDto.ManualUnderPinningServiceId = summaryViewModel?.SelectedManualUnderPinningServiceId;                          

                        }
                       
                        serviceDto.ManualUnderPinningService = new ManualUnderPinningServiceDto
                        {
                            ServiceName = summaryViewModel?.UnderPinningServiceName,
                            ProviderName = summaryViewModel?.UnderPinningProviderName,
                            CabId = summaryViewModel?.SelectCabViewModel?.SelectedCabId,
                            CertificateExpiryDate = summaryViewModel?.UnderPinningServiceExpiryDate
                        };

                    }


                }

                if (serviceDto.ServiceSupSchemeMapping != null && serviceDto.ServiceSupSchemeMapping.Count > 0)
                {


                    foreach (var serviceSchemeMapping in serviceDto.ServiceSupSchemeMapping)
                    {

                        ICollection<SchemeGPG44MappingDto>? schemeGPG44Mapping = [];
                        ICollection<SchemeGPG45MappingDto>? schemeGPG45Mapping = [];

                        //-----Gpg45/Identityprofiles--------//
                        if (summaryViewModel?.SchemeIdentityProfileMapping != null && summaryViewModel.SchemeIdentityProfileMapping.Count > 0)
                        {
                            var schemeIdentityProfileMapping = summaryViewModel.SchemeIdentityProfileMapping.Where(x => x.SchemeId == serviceSchemeMapping.SupplementarySchemeId).FirstOrDefault();
                            var selectedIdentyProfiles = schemeIdentityProfileMapping?.IdentityProfile.SelectedIdentityProfiles;
                            if (selectedIdentyProfiles != null)
                            {
                                foreach (var identityProfile in selectedIdentyProfiles)
                                {
                                    SchemeGPG45MappingDto schemeGPG45MappingDto = new()
                                    {
                                        IdentityProfileId = identityProfile.Id
                                    };
                                    schemeGPG45Mapping.Add(schemeGPG45MappingDto);
                                }
                            }
                        }

                        //-----Gpg44/auth/protection--------//
                        if (summaryViewModel?.SchemeQualityLevelMapping != null && summaryViewModel.SchemeQualityLevelMapping.Count > 0)
                        {
                            var schemeQualityLevelMapping = summaryViewModel.SchemeQualityLevelMapping.Where(x => x.SchemeId == serviceSchemeMapping.SupplementarySchemeId).FirstOrDefault();
                            serviceSchemeMapping.HasGpg44Mapping = schemeQualityLevelMapping?.HasGPG44;

                            if (serviceSchemeMapping.HasGpg44Mapping == true)
                            {
                                var selectedAuthenticatorQualityLevels = schemeQualityLevelMapping?.QualityLevel?.SelectedQualityofAuthenticators;
                                if (selectedAuthenticatorQualityLevels != null)
                                {
                                    foreach (var qualityLevel in selectedAuthenticatorQualityLevels)
                                    {
                                        SchemeGPG44MappingDto schemeGPG44MappingDto = new()
                                        {
                                            QualityLevelId = qualityLevel.Id
                                        };

                                        schemeGPG44Mapping.Add(schemeGPG44MappingDto);
                                    }
                                }

                                var selectedProtectionQualityLevels = schemeQualityLevelMapping?.QualityLevel?.SelectedLevelOfProtections;
                                if (selectedProtectionQualityLevels != null)
                                {
                                    foreach (var qualityLevel in selectedProtectionQualityLevels)
                                    {
                                        SchemeGPG44MappingDto schemeGPG44MappingDto = new()
                                        {
                                            QualityLevelId = qualityLevel.Id
                                        };
                                        schemeGPG44Mapping.Add(schemeGPG44MappingDto);
                                    }
                                }
                            }
                        }
                        serviceSchemeMapping.SchemeGPG44Mapping = schemeGPG44Mapping;
                        serviceSchemeMapping.SchemeGPG45Mapping = schemeGPG45Mapping;
                    }
                }


            }
        }
        #endregion

        public static string GetServiceType(ServiceTypeEnum? input)
        {
            if (input == ServiceTypeEnum.UnderPinning) return "Underpinning";
            else if (input == ServiceTypeEnum.WhiteLabelled) return "White-labelled";
            else return "Neither";

        }


    }
}
