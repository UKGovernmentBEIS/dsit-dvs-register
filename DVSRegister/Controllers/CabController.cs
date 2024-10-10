using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB.Provider;
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

        public CabController(ICabService cabService, IUserService userService)
        {           
            this.cabService = cabService;          
            this.userService = userService;
        }

        [HttpGet("")]
        [HttpGet("home")]
        public async Task<IActionResult> LandingPage()
        {
            string email = HttpContext?.Session.Get<string>("Email")??string.Empty; 
            string cab = string.Empty;
            var identity = HttpContext?.User.Identity as ClaimsIdentity;
            var profileClaim = identity?.Claims.FirstOrDefault(c => c.Type == "profile");
            if (profileClaim != null)
                cab = profileClaim.Value;
            CabUserDto cabUser = await userService.SaveUser(email,cab);
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
                SearchAction = InputSanitizeExtensions.CleanseInput(SearchAction);
                SearchText = InputSanitizeExtensions.CleanseInput(SearchText);
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
        public IActionResult ProviderProfileDetails(int providerId)
        {
            ProviderProfileDto providerProfileDto = HttpContext?.Session.Get<ProviderProfileDto>("ProviderProfile")??new();          
            if (providerProfileDto.Id ==  providerId)
            {
                return View(providerProfileDto);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }


        }

        [HttpGet("service-details")]
        public async Task<IActionResult> ProviderServiceDetails(int serviceId)
        {
            int cabId = Convert.ToInt32(HttpContext?.Session.Get<int>("CabId"));
            if (cabId > 0) 
            {
                ServiceDto serviceDto = await cabService.GetServiceDetails(serviceId, cabId);
                return View(serviceDto);
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }
           
        }
        #endregion

        [HttpGet("edit-company-information")]
        public IActionResult EditCompanyInformation()
        {
            return View("EditCompanyInformation");

        }


    }
}
