﻿using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB.Provider;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;


namespace DVSRegister.Controllers
{
    [Route("cab-service")]   
    public class CabController(ICabService cabService, IUserService userService, ILogger<CabController> logger) : BaseController(logger)
    {
    
        private readonly ICabService cabService = cabService;      
        private readonly IUserService userService = userService;    

        [HttpGet("")]
        [HttpGet("home")]
        public async Task<IActionResult> LandingPage()
        {
            CabUserDto cabUser = await userService.SaveUser(UserEmail, Cab);           
            if (!IsValidCabId(cabUser.CabId)) 
               return HandleInvalidCabId(cabUser.CabId);
            HttpContext?.Session.Set("CabId", cabUser.CabId); // setting logged in cab id in session
            return View();
        }



        #region Cab provider list screens
        [HttpGet("view-profiles")]
        public async Task<IActionResult> ListProviders(string SearchAction = "", string SearchText = "")
        {

            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            ProviderListViewModel providerListViewModel = new();
                if (SearchAction == "clearSearch")
                {
                    ModelState.Clear();
                    providerListViewModel.SearchText = null;
                    SearchText = string.Empty;
                }
                providerListViewModel.Providers = await cabService.GetProviders(CabId, SearchText);
                return View(providerListViewModel);

            
          
        }

        [HttpGet("profile-overview")]
        public async Task<IActionResult> ProviderOverview(int providerId)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
            HttpContext?.Session.Remove("ProviderProfile");// clear existing data if any
            HttpContext?.Session.Set("ProviderProfile", providerProfileDto);
            return View(providerProfileDto);
        }
        
        [HttpGet("profile-information")]
        public async Task<IActionResult> ProviderProfileDetails(int providerId)
        {
                    

            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            ProviderProfileDto providerProfileDto = await cabService.GetProvider(providerId, CabId);
            HttpContext?.Session.Remove("ProviderProfile");// clear existing data if any
            HttpContext?.Session.Set("ProviderProfile", providerProfileDto);
            ProviderDetailsViewModel providerDetailsViewModel = new()
            {
                Provider = providerProfileDto,
                IsCompanyInfoEditable =  cabService.CheckCompanyInfoEditable(providerProfileDto)
            };

            return View(providerDetailsViewModel);
        }

        [HttpGet("service-details")]
        public async Task<IActionResult> ProviderServiceDetails(int serviceKey)
        {
            HttpContext?.Session.Remove("ServiceSummary");
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            
            ServiceVersionViewModel serviceVersions = new();
            var serviceList = await cabService.GetServiceList(serviceKey, CabId);
            ServiceDto currentServiceVersion = serviceList?.FirstOrDefault(x => x.IsCurrent == true) ?? new ServiceDto();
            serviceVersions.CurrentServiceVersion = currentServiceVersion;
            serviceVersions.ServiceHistoryVersions = serviceList?.Where(x => x.IsCurrent != true).OrderByDescending(x=> x.PublishedTime).ToList()?? new ();

            SetServiceDataToSession(CabId, serviceVersions.CurrentServiceVersion, serviceVersions.ServiceHistoryVersions.Count);
            return View(serviceVersions);
        }


        #endregion
    }
}