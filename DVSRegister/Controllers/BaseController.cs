using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DVSRegister.Controllers
{


    [ValidCognitoToken]
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
        protected string UserEmail => HttpContext.Session.Get<string>("Email") ?? string.Empty;
        protected int CabId => HttpContext.Session.Get<int>("CabId");

        protected string ControllerName => ControllerContext.ActionDescriptor.ControllerName;
        protected string ActionName => ControllerContext.ActionDescriptor.ActionName;
        protected string Cab
        {
            get
            {
                var identity = HttpContext?.User.Identity as ClaimsIdentity;
                var profileClaim = identity?.Claims.FirstOrDefault(c => c.Type == "profile");
                return profileClaim?.Value ?? string.Empty;
            }
        }

        protected bool IsValidCabId(int cabId)
        {
            return cabId > 0;
        }

        protected void SetRefererURL()
        {
            string refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            ViewBag.RefererUrl = refererUrl;
        }

        protected string GetRefererURL()
        {
            string refererUrl = HttpContext.Request.Headers["Referer"].ToString();
            return refererUrl;
        }



        protected IActionResult HandleInvalidCabId(int cabId)
        {
            _logger.LogError("Invalid CabId: {CabId}. Controller: {ControllerName}, Action: {ActionName}",
                cabId, ControllerName, ActionName);
            return RedirectToAction("CabHandleException", "Error");
        }



        protected ServiceSummaryViewModel GetServiceSummary()
        {

            ServiceSummaryViewModel model = HttpContext?.Session.Get<ServiceSummaryViewModel>("ServiceSummary") ?? new ServiceSummaryViewModel
            {
                QualityLevelViewModel = new QualityLevelViewModel { SelectedLevelOfProtections = new List<QualityLevelDto>(), SelectedQualityofAuthenticators = new List<QualityLevelDto>() },
                RoleViewModel = new RoleViewModel { SelectedRoles = new List<RoleDto>() },
                TFVersionViewModel = new TFVersionViewModel { SelectedTFVersion = new TrustFrameworkVersionDto() },
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = new List<IdentityProfileDto>() },
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } },
                SchemeIdentityProfileMapping = new List<SchemeIdentityProfileMappingViewModel>(),
                SchemeQualityLevelMapping = new List<SchemeQualityLevelMappingViewModel> { }
            };
            return model;
        }


        protected void SetServiceDataToSession(int cabId, ServiceDto serviceDto, bool isAmendment = false)
        {
            TFVersionViewModel TFVersionViewModel = new()
            {
                SelectedTFVersion = null
            };
            RoleViewModel roleViewModel = new()
            {
                SelectedRoles = []
            };
            QualityLevelViewModel qualityLevelViewModel = new()
            {
                SelectedLevelOfProtections = [],
                SelectedQualityofAuthenticators = []
            };

            IdentityProfileViewModel identityProfileViewModel = new()
            {
                SelectedIdentityProfiles = []
            };

            SupplementarySchemeViewModel supplementarySchemeViewModel = new()
            {
                SelectedSupplementarySchemes = []
            };
            SelectCabViewModel selectCabViewModel = new()
            {
                SelectedCabId = null,
                SelectedCabName = null
            };
            List<SchemeIdentityProfileMappingViewModel> schemeIdentityProfileMappingViewModelList = [];
            List<SchemeQualityLevelMappingViewModel> schemeQualityLevelMappingViewModelList = [];
          

            if (serviceDto.TrustFrameworkVersion != null)
            {
                TFVersionViewModel.SelectedTFVersion = serviceDto.TrustFrameworkVersion;
            }
            if (serviceDto.ServiceRoleMapping != null && serviceDto.ServiceRoleMapping.Count > 0)
            {
                roleViewModel.SelectedRoles = serviceDto.ServiceRoleMapping.Select(mapping => mapping.Role).ToList();
            }

            if (serviceDto.ServiceQualityLevelMapping != null && serviceDto.ServiceQualityLevelMapping.Count > 0)
            {
                var protectionLevels = serviceDto.ServiceQualityLevelMapping
                .Where(item => item.QualityLevel.QualityType == QualityTypeEnum.Protection)
                .Select(item => item.QualityLevel);

                var authenticatorLevels = serviceDto.ServiceQualityLevelMapping
                .Where(item => item.QualityLevel.QualityType == QualityTypeEnum.Authentication)
                .Select(item => item.QualityLevel);

                foreach (var item in protectionLevels)
                {
                    qualityLevelViewModel.SelectedLevelOfProtections.Add(item);
                }

                foreach (var item in authenticatorLevels)
                {
                    qualityLevelViewModel.SelectedQualityofAuthenticators.Add(item);
                }


            }
            if (serviceDto.ServiceIdentityProfileMapping != null && serviceDto.ServiceIdentityProfileMapping.Count > 0)
            {
                identityProfileViewModel.SelectedIdentityProfiles = serviceDto.ServiceIdentityProfileMapping.Select(mapping => mapping.IdentityProfile).ToList();
            }
            if (serviceDto.ServiceSupSchemeMapping != null && serviceDto.ServiceSupSchemeMapping.Count > 0)
            {
                supplementarySchemeViewModel.SelectedSupplementarySchemes = serviceDto.ServiceSupSchemeMapping.Select(mapping => mapping.SupplementaryScheme).ToList();
                HttpContext?.Session.Set("SelectedSchemeIds", supplementarySchemeViewModel.SelectedSupplementarySchemes.Select(scheme => scheme.Id).ToList());
                foreach (var item in serviceDto.ServiceSupSchemeMapping)
                {                    
                    
                    if(item.SchemeGPG45Mapping != null && item.SchemeGPG45Mapping.Count>0)
                    {
                        SchemeIdentityProfileMappingViewModel schemeIdentityProfileMappingViewModel = new();
                        schemeIdentityProfileMappingViewModel.SchemeId = item.SupplementarySchemeId;
                        schemeIdentityProfileMappingViewModel.IdentityProfile = new IdentityProfileViewModel
                        {
                            SelectedIdentityProfiles = item.SchemeGPG45Mapping.Select(mapping => mapping.IdentityProfile).ToList()
                        };

                        schemeIdentityProfileMappingViewModelList.Add(schemeIdentityProfileMappingViewModel);
                    }

                    if (item.HasGpg44Mapping != null)
                    {
                        SchemeQualityLevelMappingViewModel schemeQualityLevelMappingViewModel = new();
                        schemeQualityLevelMappingViewModel.HasGPG44 = item.HasGpg44Mapping;
                        schemeQualityLevelMappingViewModel.SchemeId = item.SupplementarySchemeId;
                        if (item.SchemeGPG44Mapping != null && item.SchemeGPG44Mapping.Count > 0)
                        {                           
                                                     
                            schemeQualityLevelMappingViewModel.QualityLevel = new QualityLevelViewModel
                            {
                                SelectedQualityofAuthenticators = item.SchemeGPG44Mapping.Select(mapping => mapping.QualityLevel).Where(x => x.QualityType == QualityTypeEnum.Authentication).ToList(),
                                SelectedLevelOfProtections = item.SchemeGPG44Mapping.Select(mapping => mapping.QualityLevel).Where(x => x.QualityType == QualityTypeEnum.Protection).ToList(),
                            };                           
                        }
                        schemeQualityLevelMappingViewModelList.Add(schemeQualityLevelMappingViewModel);
                    }                   

                }
            }

            if (serviceDto?.ManualUnderPinningService?.Cab != null)
            {
                selectCabViewModel.SelectedCabId = serviceDto.ManualUnderPinningService.Cab.Id;
                selectCabViewModel.SelectedCabName = serviceDto.ManualUnderPinningService.Cab.CabName;
            }
            else if (serviceDto?.UnderPinningService != null)
            {
                selectCabViewModel.SelectedCabId = serviceDto.UnderPinningService.CabUser.Cab.Id;
                selectCabViewModel.SelectedCabName = serviceDto.UnderPinningService.CabUser.Cab.CabName;
            }


            ServiceSummaryViewModel serviceSummary = new()
            {
                TFVersionViewModel = TFVersionViewModel,
                ServiceName = serviceDto.ServiceName,
                ServiceURL = serviceDto.WebSiteAddress,
                CompanyAddress = serviceDto.CompanyAddress,
                RoleViewModel = roleViewModel,
                IdentityProfileViewModel = identityProfileViewModel,
                QualityLevelViewModel = qualityLevelViewModel,
                SelectCabViewModel = selectCabViewModel,
                HasSupplementarySchemes = serviceDto.HasSupplementarySchemes,
                ServiceType = serviceDto?.ServiceType ?? 0,
                IsUnderpinningServicePublished = serviceDto?.IsUnderPinningServicePublished,
                SelectedManualUnderPinningServiceId = serviceDto?.ManualUnderPinningServiceId,//non published manual
                IsManualServiceLinkedToMultipleServices = serviceDto?.IsManualServiceLinkedToMultipleServices, // non published manual selected from list
                SelectedUnderPinningServiceId = serviceDto?.UnderPinningServiceId,// published

                UnderPinningServiceName = serviceDto?.UnderPinningServiceId == null? serviceDto?.ManualUnderPinningService?.ServiceName : serviceDto?.UnderPinningService?.ServiceName,
                UnderPinningProviderName = serviceDto?.UnderPinningServiceId == null ? serviceDto?.ManualUnderPinningService?.ProviderName: serviceDto?.UnderPinningService.Provider?.RegisteredName,
                UnderPinningServiceExpiryDate = serviceDto?.UnderPinningServiceId == null ? serviceDto?.ManualUnderPinningService?.CertificateExpiryDate: serviceDto?.UnderPinningService.ConformityExpiryDate,

                HasGPG44 = serviceDto.HasGPG44,
                HasGPG45 = serviceDto.HasGPG45,
                SupplementarySchemeViewModel = supplementarySchemeViewModel,
                SchemeIdentityProfileMapping = schemeIdentityProfileMappingViewModelList,
                SchemeQualityLevelMapping = schemeQualityLevelMappingViewModelList,
                FileLink = serviceDto.FileLink,
                FileName = serviceDto.FileName,
                FileSizeInKb = serviceDto.FileSizeInKb,
                ConformityIssueDate = serviceDto.ConformityIssueDate == DateTime.MinValue ? null : serviceDto.ConformityIssueDate,
                ConformityExpiryDate = serviceDto.ConformityExpiryDate == DateTime.MinValue ? null : serviceDto.ConformityExpiryDate,
                ServiceId = serviceDto.Id,
                ProviderProfileId = serviceDto.ProviderProfileId,
                CabId = cabId,
                CabUserId = serviceDto.CabUserId,
                ServiceKey = serviceDto.ServiceKey,
                IsDraft = serviceDto.ServiceStatus == ServiceStatusEnum.SavedAsDraft,
                IsResubmission = serviceDto.ServiceVersion >1, // if there are previous versions , it means a resubmission
                IsAmendment = isAmendment,
                ServiceStatus = serviceDto.ServiceStatus,
                CreatedDate = serviceDto.CreatedTime
            };
            HttpContext?.Session.Set("ServiceSummary", serviceSummary);
        }


    }
}