﻿using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Provider;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DVSRegister.Controllers
{
    [Route("cab-service")]
    [ValidCognitoToken]
    public class CabController : Controller
    {
    
        private readonly ICabService cabService;      
        private readonly IUserService userService;
        private string UserEmail => HttpContext.Session.Get<string>("Email")??string.Empty;
        public CabController(ICabService cabService, IUserService userService)
        {           
            this.cabService = cabService;          
            this.userService = userService;
        }

        [HttpGet("")]
        [HttpGet("home")]
        public async Task<IActionResult> LandingPage()
        {
           
            string cab = string.Empty;
            var identity = HttpContext?.User.Identity as ClaimsIdentity;
            var profileClaim = identity?.Claims.FirstOrDefault(c => c.Type == "profile");
            if (profileClaim != null)
                cab = profileClaim.Value;
            CabUserDto cabUser = await userService.SaveUser(UserEmail,cab);
            HttpContext?.Session.Set("CabId", cabUser.CabId); // setting logged in cab id in session
            return cabUser.CabId>0 ? View() : RedirectToAction("HandleException", "Error");
           
        }



        #region Cab provider list screens
        [HttpGet("view-profiles")]
        public async Task<IActionResult> ListProviders(string SearchAction = "", string SearchText = "")
        {
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));

            if (cabId > 0)
            {
                ProviderListViewModel providerListViewModel = new();
                if (SearchAction == "clearSearch")
                {
                    ModelState.Clear();
                    providerListViewModel.SearchText = null;
                    SearchText = string.Empty;
                }
                providerListViewModel.Providers = await cabService.GetProviders(cabId, SearchText);
                return View(providerListViewModel);

            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
          
        }

        [HttpGet("profile-overview")]
        public async Task<IActionResult> ProviderOverview(int providerId)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));

            if (cabId > 0) 
            {
                ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, cabId);
                HttpContext?.Session.Remove("ProviderProfile");// clear existing data if any
                HttpContext?.Session.Set("ProviderProfile", providerProfileDto);
                return View(providerProfileDto);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
           
        }
        [HttpGet("profile-information")]
        public async Task<IActionResult> ProviderProfileDetails(int providerId)
        {
          
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));

            if (cabId > 0)
            {
                ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, cabId);
                HttpContext?.Session.Remove("ProviderProfile");// clear existing data if any
                HttpContext?.Session.Set("ProviderProfile", providerProfileDto);
                ProviderDetailsViewModel providerDetailsViewModel = new()
                {
                    Provider = providerProfileDto,
                    IsCompanyInfoEditable =  cabService.CheckCompanyInfoEditable(providerProfileDto)
                };

                return View(providerDetailsViewModel);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }


        }

        [HttpGet("service-details")]
        public async Task<IActionResult> ProviderServiceDetails(int serviceKey)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0)
            {
                ServiceVersionViewModel serviceVersions = new();
                var serviceList = await cabService.GetServiceList(serviceKey, cabId);
                ServiceDto currentServiceVersion = serviceList?.FirstOrDefault(x => x.IsCurrent == true) ?? new ServiceDto();
                serviceVersions.CurrentServiceVersion = currentServiceVersion;
                serviceVersions.ServiceHistoryVersions = serviceList?.Where(x => x.IsCurrent != true).OrderByDescending(x=> x.PublishedTime).ToList()?? new ();

                SetServiceDataToSession(cabId, serviceVersions.CurrentServiceVersion, serviceVersions.ServiceHistoryVersions.Count);
                return View(serviceVersions);

            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
           
        }


        #endregion

        #region Private method
        private void SetServiceDataToSession(int cabId, ServiceDto serviceDto, int historyCount)
        {
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
                IsResubmission = historyCount >0 // if there are previous versions , it means a resubmission
            };
            HttpContext?.Session.Set("ServiceSummary", serviceSummary);
        }
        #endregion





    }
}
