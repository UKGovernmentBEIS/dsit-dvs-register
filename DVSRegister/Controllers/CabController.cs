using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Controllers
{
    [Route("cab-registration")]
    public class CabController : Controller
    {
    
        private readonly ICabService cabService;

        public CabController(ICabService cabService)
        {           
            this.cabService = cabService;
        }

        [HttpGet("landing-page")]
        public IActionResult LandingPage()
        {
            
            return View();
        }

        /// <summary>
        /// Summary page displaying data saved in session
        /// </summary>      
        /// <returns></returns>
        [HttpGet("check-your-answers")]
        public IActionResult CertificateInfoSummary()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();          
            return View(summaryViewModel);
        }

        /// <summary>
        /// Get data from session, convert to dto and save
        /// to database on Confirm click
        /// </summary>
        /// <param name="summaryViewModel"></param>
        /// <returns></returns>

        [HttpPost("check-your-answers")]
        public async Task<IActionResult> SaveSummaryAndSubmit()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            CertificateInfoDto certificateInfoDto = MapViewModelToDto(summaryViewModel);
            GenericResponse genericResponse = await cabService.SaveCertificateInformation(certificateInfoDto);
            if (genericResponse.Success)
            {
                return RedirectToAction("InformationSubmitted");
            }
            else
            {
                return RedirectToAction("HandleException", "Error");
            }           

        }

        /// <summary>
        ///Final page if save success
        /// </summary>       
        /// <returns></returns>
        [HttpGet("information-submitted")]
        public IActionResult InformationSubmitted()
        {
            CertificateInfoSummaryViewModel summaryViewModel = GetCertificateInfoSummary();
            return View(summaryViewModel);
        }

        /// <summary>
        ///Return to landing page
        /// </summary>       
        /// <returns></returns>
        [HttpPost("return-to-service")]
        public IActionResult ReturnToService()
        {
            return RedirectToAction("LandingPage");
        }


        #region Private methods
        private CertificateInfoSummaryViewModel GetCertificateInfoSummary()
        {
            CertificateInfoSummaryViewModel model = HttpContext?.Session.Get<CertificateInfoSummaryViewModel>("CertificateInfoSummary") ?? new CertificateInfoSummaryViewModel
            {
                RoleViewModel = new RoleViewModel { SelectedRoles = new List<RoleDto>() },
                IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = new List<IdentityProfileDto>()},
                SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = new List<SupplementarySchemeDto> { } }
            };
            return model;
        }

        private CertificateInfoDto MapViewModelToDto(CertificateInfoSummaryViewModel model)
        {
            CertificateInfoDto certificateInfoDto = new CertificateInfoDto();
            ICollection<CertificateInfoRoleMappingDto> certificateInfoRoleMappings = new List<CertificateInfoRoleMappingDto>();
            ICollection<CertificateInfoIdentityProfileMappingDto> certificateInfoIdentityProfileMappings = new List<CertificateInfoIdentityProfileMappingDto>();
            ICollection<CertificateInfoSupSchemeMappingDto> certificateInfoSupSchemeMappings = new List<CertificateInfoSupSchemeMappingDto>();
            foreach (var item in model.RoleViewModel.SelectedRoles)
            {
                certificateInfoRoleMappings.Add(new CertificateInfoRoleMappingDto { RoleId = item.Id });
            }
            foreach (var item in model.IdentityProfileViewModel.SelectedIdentityProfiles)
            {
                certificateInfoIdentityProfileMappings.Add(new CertificateInfoIdentityProfileMappingDto { IdentityProfileId = item.Id });
            }
            foreach (var item in model.SupplementarySchemeViewModel.SelectedSupplementarySchemes)
            {
                certificateInfoSupSchemeMappings.Add(new CertificateInfoSupSchemeMappingDto { SupplementarySchemeId = item.Id });
            }
            certificateInfoDto.PreRegistrationId= Convert.ToInt32(model.PreRegistrationId);
            certificateInfoDto.RegisteredName = model.RegisteredName??string.Empty;
            certificateInfoDto.TradingName = model.TradingName??string.Empty;
            certificateInfoDto.PublicContactEmail = model.PublicContactEmail??string.Empty;
            certificateInfoDto.TelephoneNumber = model.TelephoneNumber??string.Empty;
            certificateInfoDto.WebsiteAddress = model.WebsiteAddress??string.Empty;
            certificateInfoDto.Address = model.Address??string.Empty;
            certificateInfoDto.ServiceName = model.ServiceName??string.Empty;
            certificateInfoDto.CertificateInfoRoleMappings = certificateInfoRoleMappings;
            certificateInfoDto.CertificateInfoIdentityProfileMappings = certificateInfoIdentityProfileMappings;
            certificateInfoDto.HasSupplementarySchemes = Convert.ToBoolean(model.HasSupplementarySchemes);
            certificateInfoDto.CertificateInfoSupSchemeMappings = certificateInfoSupSchemeMappings;
            certificateInfoDto.FileName = model.FileName??string.Empty;
            certificateInfoDto.FileLink = model.FileLink?? string.Empty;
            certificateInfoDto.ConformityIssueDate = Convert.ToDateTime(model.ConformityIssueDate);
            certificateInfoDto.ConformityExpiryDate = Convert.ToDateTime(model.ConformityExpiryDate);
            certificateInfoDto.CreatedDate = DateTime.UtcNow;
            certificateInfoDto.CertificateInfoStatus = CertificateInfoStatusEnum.Received;
            return certificateInfoDto;

        }
        #endregion
    }
}
