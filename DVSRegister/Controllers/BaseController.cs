using Amazon.S3.Model.Internal.MarshallTransformations;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
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

        //TODO: set value in session 
        protected Decimal TFVersionNumber => 0.3m;

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
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } }
            };
            return model;
        }


        protected void SetServiceDataToSession(int cabId, ServiceDto serviceDto, int historyCount = 0)
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
                HasSupplementarySchemes = serviceDto.HasSupplementarySchemes,
                HasGPG44 = serviceDto.HasGPG44,
                HasGPG45 = serviceDto.HasGPG45,
                SupplementarySchemeViewModel = supplementarySchemeViewModel,
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
                IsResubmission = historyCount > 0, // if there are previous versions , it means a resubmission
                IsAmendment = serviceDto.ServiceStatus == ServiceStatusEnum.AmendmentsRequired,
                ServiceStatus = serviceDto.ServiceStatus,
                CreatedDate = serviceDto.CreatedTime
            };
            HttpContext?.Session.Set("ServiceSummary", serviceSummary);
        }


    }
}