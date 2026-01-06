using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models.CAB;

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
                summaryViewModel.SchemeQualityLevelMapping.ForEach(item =>
                {
                    if (item.QualityLevel != null)
                    {
                        item.QualityLevel.SelectedQualityofAuthenticators = [];
                        item.QualityLevel.SelectedLevelOfProtections = [];
                    }
                });

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
                            // Initialize the nested DTO ONLY in amendment flow
                            if(summaryViewModel?.IsAmendment == true)
                            {
                                serviceDto.ManualUnderPinningService = new ManualUnderPinningServiceDto
                                {
                                    ServiceName = summaryViewModel?.UnderPinningServiceName,
                                    ProviderName = summaryViewModel?.UnderPinningProviderName,
                                    CabId = summaryViewModel?.SelectCabViewModel?.SelectedCabId,
                                    CertificateExpiryDate = summaryViewModel?.UnderPinningServiceExpiryDate
                                };
                            }

                        }
                        else
                        {
                            // Create a new underpinning service when id is null, which means it is created for first time
                            serviceDto.ManualUnderPinningService = new ManualUnderPinningServiceDto
                            {
                                ServiceName = summaryViewModel?.UnderPinningServiceName,
                                ProviderName = summaryViewModel?.UnderPinningProviderName,
                                CabId = summaryViewModel?.SelectCabViewModel?.SelectedCabId,
                                CertificateExpiryDate = summaryViewModel?.UnderPinningServiceExpiryDate
                            };
                        }
                       
                        

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

        public static ProviderProfileDto MapViewModelToDto(ProfileSummaryViewModel model, int cabUserId, int cabId)
        {
            ProviderProfileDto providerDto = null;
            if (model != null && !string.IsNullOrEmpty(model.RegisteredName) && model.HasRegistrationNumber != null &&
                model.HasParentCompany != null
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactFullName) &&
                !string.IsNullOrEmpty(model?.PrimaryContact.PrimaryContactJobTitle)
                && !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactEmail) &&
                !string.IsNullOrEmpty(model.PrimaryContact?.PrimaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactFullName) &&
                !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactJobTitle)
                && !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactEmail) &&
                !string.IsNullOrEmpty(model.SecondaryContact?.SecondaryContactTelephoneNumber)
                && !string.IsNullOrEmpty(model.ProviderWebsiteAddress) && cabUserId > 0)
            {
                providerDto = new();
                providerDto.ProviderProfileCabMapping = [new ProviderProfileCabMappingDto { CabId = cabId }];
                providerDto.RegisteredName = model.RegisteredName;
                providerDto.TradingName = model.TradingName ?? string.Empty;
                providerDto.HasRegistrationNumber = model.HasRegistrationNumber ?? false;
                providerDto.CompanyRegistrationNumber = model.CompanyRegistrationNumber;
                providerDto.DUNSNumber = model.DUNSNumber;
                providerDto.HasParentCompany = model.HasParentCompany ?? false;
                providerDto.ParentCompanyRegisteredName = model.ParentCompanyRegisteredName;
                providerDto.ParentCompanyLocation = model.ParentCompanyLocation;
                providerDto.PrimaryContactFullName = model.PrimaryContact.PrimaryContactFullName;
                providerDto.PrimaryContactJobTitle = model.PrimaryContact.PrimaryContactJobTitle;
                providerDto.PrimaryContactEmail = model.PrimaryContact.PrimaryContactEmail;
                providerDto.PrimaryContactTelephoneNumber = model.PrimaryContact.PrimaryContactTelephoneNumber;
                providerDto.SecondaryContactFullName = model.SecondaryContact.SecondaryContactFullName;
                providerDto.SecondaryContactJobTitle = model.SecondaryContact.SecondaryContactJobTitle;
                providerDto.SecondaryContactEmail = model.SecondaryContact.SecondaryContactEmail;
                providerDto.SecondaryContactTelephoneNumber = model.SecondaryContact.SecondaryContactTelephoneNumber;
                providerDto.PublicContactEmail = model.PublicContactEmail;
                providerDto.ProviderTelephoneNumber = model.ProviderTelephoneNumber;
                providerDto.ProviderWebsiteAddress = model.ProviderWebsiteAddress;
                providerDto.LinkToContactPage = model.LinkToContactPage;
                providerDto.ProviderProfileCabMapping = [new ProviderProfileCabMappingDto { CabId = cabId }];
                providerDto.ProviderStatus = ProviderStatusEnum.NA;
                providerDto.CreatedTime = DateTime.UtcNow;
                providerDto.Id = model.ProviderId;
            }


            return providerDto;
        }

      
        public static InProgressApplicationParameters GetInProgressApplicationParameters(List<ServiceDto>? serviceList)
        {

            if (serviceList!= null && serviceList.Count > 0)
            {
                InProgressApplicationParameters inProgressApplicationParameters = new();
                ServiceDto inprogressService = serviceList.Where(x => 
                (x.ServiceStatus == ServiceStatusEnum.Submitted || x.ServiceStatus == ServiceStatusEnum.Received ||
                x.ServiceStatus == ServiceStatusEnum.Resubmitted || x.ServiceStatus == ServiceStatusEnum.AmendmentsRequired) &&
                x.CertificateReview.Where(x => x.IsLatestReviewVersion).SingleOrDefault()?.CertificateReviewStatus != CertificateReviewEnum.Rejected &&
                x.PublicInterestCheck.Where(x => x.IsLatestReviewVersion).SingleOrDefault()?.PublicInterestCheckStatus != PublicInterestCheckEnum.PublicInterestCheckFailed).FirstOrDefault() ?? new();
                inProgressApplicationParameters.InProgressApplicationId = inprogressService.Id;
                inProgressApplicationParameters.HasInProgressApplication = inProgressApplicationParameters.InProgressApplicationId > 0;
                inProgressApplicationParameters.ServiceId = inprogressService.Id;
                ServiceDto reassginmentRequestService = serviceList?.Where(x => x.ServiceStatus == ServiceStatusEnum.RemovedUnderReassign || x.ServiceStatus == ServiceStatusEnum.PublishedUnderReassign).FirstOrDefault() ?? null!;
                if (reassginmentRequestService != null && reassginmentRequestService.Id > 0)
                {
                    inProgressApplicationParameters.HasActiveReassignmentRequest = true;
                    inProgressApplicationParameters.InProgressReassignmentRequestServiceId = reassginmentRequestService.Id;
                    inProgressApplicationParameters.ServiceId = inProgressApplicationParameters.ServiceId == 0? reassginmentRequestService.Id : inProgressApplicationParameters.ServiceId;


                }

                ServiceDto removalrequestService = serviceList?.Where(x => x.ServiceStatus == ServiceStatusEnum.CabAwaitingRemovalConfirmation || x.ServiceStatus == ServiceStatusEnum.AwaitingRemovalConfirmation).FirstOrDefault() ?? null!;
                if (removalrequestService != null && removalrequestService.Id > 0)
                {
                    inProgressApplicationParameters.HasActiveRemovalRequest = true;
                    inProgressApplicationParameters.InProgressRemovalRequestServiceId = removalrequestService.Id;
                    inProgressApplicationParameters.ServiceId = inProgressApplicationParameters.ServiceId == 0 ? removalrequestService.Id : inProgressApplicationParameters.ServiceId;
                }


               

                ServiceDto inprogresAndUpdateRequestedService = serviceList?.Where(x => x.ServiceStatus == ServiceStatusEnum.UpdatesRequested &&
                x?.serviceDraft?.PreviousServiceStatus != ServiceStatusEnum.Published && (x?.serviceDraft?.PreviousServiceStatus == ServiceStatusEnum.Submitted
                || x?.serviceDraft?.PreviousServiceStatus == ServiceStatusEnum.Received ||
                x?.serviceDraft?.PreviousServiceStatus == ServiceStatusEnum.Resubmitted ||
                x?.serviceDraft?.PreviousServiceStatus == ServiceStatusEnum.AmendmentsRequired)).FirstOrDefault()??null!;
                if(inprogresAndUpdateRequestedService!=null && inprogresAndUpdateRequestedService.Id > 0)
                {
                    inProgressApplicationParameters.InProgressAndUpdateRequested = true;
                    inProgressApplicationParameters.InProgressAndUpdateRequestedId = inprogresAndUpdateRequestedService.Id;
                    inProgressApplicationParameters.ServiceId = inprogresAndUpdateRequestedService.Id;
                }
                



                List<ServiceDto> updateRequestServices = inProgressApplicationParameters.InProgressAndUpdateRequested ?
                serviceList?.Where(x => x.ServiceStatus == ServiceStatusEnum.UpdatesRequested && x.IsCurrent == false).ToList() ?? null! :
                serviceList?.Where(x => x.ServiceStatus == ServiceStatusEnum.UpdatesRequested ).ToList() ?? null!;
                if (updateRequestServices != null && updateRequestServices.Count > 0)
                {
                    inProgressApplicationParameters.HasActiveUpdateRequest = true;
                    inProgressApplicationParameters.InProgressUpdateRequestServiceIds = [];
                    foreach (var service in updateRequestServices)
                    {
                        inProgressApplicationParameters.InProgressUpdateRequestServiceIds.Add(service.Id);
                    }

                }

                return inProgressApplicationParameters;

            }
            else
            {
                return null!;
            }

        }


    }
}
