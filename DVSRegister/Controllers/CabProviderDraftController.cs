using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    public class CabProviderDraftController(ILogger<BaseController> logger) :BaseController(logger)
    {
       

        [HttpGet("complete-provider-profile")]
        public IActionResult CompleteProviderProfile()
        {
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);
            ProfileSummaryViewModel profileSummary = HttpContext?.Session.Get<ProfileSummaryViewModel>("ProfileSummary") ?? new ProfileSummaryViewModel();
            return RedirectToNextEmptyField(profileSummary);

        }

        private IActionResult RedirectToNextEmptyField(ProfileSummaryViewModel profileSummary)
        {
            if (string.IsNullOrEmpty(profileSummary.RegisteredName))
            {
                return RedirectToAction("RegisteredName", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (string.IsNullOrEmpty(profileSummary.TradingName) && profileSummary.HasRegistrationNumber == null)
            {
                return RedirectToAction("TradingName", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (profileSummary.HasRegistrationNumber == null)
            {
                return RedirectToAction("HasRegistrationNumber", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }

            else if (profileSummary.HasRegistrationNumber != null && profileSummary.HasRegistrationNumber == true && string.IsNullOrEmpty(profileSummary.CompanyRegistrationNumber))
            {
                return RedirectToAction("CompanyRegistrationNumber", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (profileSummary.HasRegistrationNumber != null && profileSummary.HasRegistrationNumber == false && string.IsNullOrEmpty(profileSummary.DUNSNumber))
            {
                return RedirectToAction("DUNSNumber", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (profileSummary.HasParentCompany == null)
            {
                return RedirectToAction("HasParentCompany", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }         

            else if (profileSummary.HasParentCompany == true && string.IsNullOrEmpty(profileSummary.ParentCompanyRegisteredName))
            {
                return RedirectToAction("ParentCompanyRegisteredName", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (profileSummary.HasParentCompany == true && string.IsNullOrEmpty(profileSummary.ParentCompanyLocation))
            {
                return RedirectToAction("ParentCompanyLocation", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (HasMissingPrimaryContactInfo(profileSummary.PrimaryContact!))
            {
                return RedirectToAction("PrimaryContact", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (HasMissingSecondaryContactInfo(profileSummary.SecondaryContact!))
            {
                return RedirectToAction("SecondaryContact", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (string.IsNullOrEmpty(profileSummary.PublicContactEmail) && string.IsNullOrEmpty(profileSummary.ProviderTelephoneNumber) 
                && string.IsNullOrEmpty(profileSummary.ProviderWebsiteAddress))
            {
                return RedirectToAction("PublicContactEmail", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (string.IsNullOrEmpty(profileSummary.ProviderTelephoneNumber) && string.IsNullOrEmpty(profileSummary.ProviderWebsiteAddress))
            {
                return RedirectToAction("TelephoneNumber", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (string.IsNullOrEmpty(profileSummary.ProviderWebsiteAddress))
            {
                return RedirectToAction("WebsiteAddress", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else if (string.IsNullOrEmpty(profileSummary.LinkToContactPage))
            {
                return RedirectToAction("LinkToContactPage", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }
            else
            {
                return RedirectToAction("ProfileSummary", "CabProvider", new { sourcePage = SourcePageEnum.profiledraft });
            }

        }


        private bool HasMissingPrimaryContactInfo(PrimaryContactViewModel model)
        {
            var properties = new[]
            {
                model.PrimaryContactFullName,
                model.PrimaryContactJobTitle,
                model.PrimaryContactEmail,
                model.PrimaryContactTelephoneNumber
            };

            return properties.Any(string.IsNullOrWhiteSpace);
        }
        private bool HasMissingSecondaryContactInfo(SecondaryContactViewModel model)
        {
            var properties = new[]
            {
                model.SecondaryContactFullName,
                model.SecondaryContactJobTitle,
                model.SecondaryContactEmail,
                model.SecondaryContactTelephoneNumber
            };

            return properties.Any(string.IsNullOrWhiteSpace);
        }

    }
}
