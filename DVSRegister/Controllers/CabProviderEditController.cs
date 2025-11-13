using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.Edit;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    [Route("cab-service/provider-profile")]
    public class CabProviderEditController(IEditService editService,  IUserService userService, ILogger<CabController> logger) : BaseController(logger)
    {
        private readonly IEditService editService = editService;
        private readonly IUserService userService = userService;



        [HttpGet("change-provider-details")]
        public IActionResult ProfileEditSummary()
        {          
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            return View(profileSummaryViewModel);
        }


        [HttpGet("edit-summary")]
        public async Task<IActionResult> ProviderDifference()
        {
            ProviderChangesViewModel changesViewModel = new();
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ProviderProfileDto currentProvider = await editService.GetProviderDetails(profileSummaryViewModel.ProviderId);
            ProviderProfileDraftDto changedProvider = CreateDraft(currentProvider, profileSummaryViewModel);
            List<string> dsitEmails = await userService.GetDSITUserEmails();
            changesViewModel.DSITUserEmails = string.Join(",", dsitEmails);
            changesViewModel.CurrentProvider = currentProvider;
            changesViewModel.ChangedProvider = changedProvider;

            (changesViewModel.PreviousDataKeyValuePair, changesViewModel.CurrentDataKeyValuePair) = await editService.GetProviderKeyValue(changedProvider, currentProvider);

            HttpContext?.Session.Set("changedProvider", changedProvider);

            return View(changesViewModel);
        }

        private ProfileSummaryViewModel GetProfileSummary()
        {
            ProfileSummaryViewModel model = HttpContext?.Session.Get<ProfileSummaryViewModel>("ProfileSummary") ??
                                            new ProfileSummaryViewModel
                                            {
                                                PrimaryContact = new PrimaryContactViewModel(),
                                                SecondaryContact = new SecondaryContactViewModel()
                                            };
            return model;
        }

        private ProviderProfileDraftDto CreateDraft(ProviderProfileDto existingProvider, ProfileSummaryViewModel updatedService)
        {

            var draft = new ProviderProfileDraftDto
            {
                ProviderProfileId = existingProvider.Id,
                PreviousProviderStatus = existingProvider.ProviderStatus
            };

            draft.RegisteredName = updatedService.RegisteredName != existingProvider.RegisteredName ? updatedService.RegisteredName : null;
            draft.TradingName = updatedService.TradingName != existingProvider.TradingName ? (updatedService.TradingName ?? "-") : null;
            draft.CompanyRegistrationNumber = updatedService.CompanyRegistrationNumber != existingProvider.CompanyRegistrationNumber ?
                (updatedService.CompanyRegistrationNumber ?? "-") : null;
            draft.HasRegistrationNumber = updatedService.HasRegistrationNumber != existingProvider.HasRegistrationNumber ? updatedService.HasRegistrationNumber : null;
            draft.DUNSNumber = updatedService.DUNSNumber != existingProvider.DUNSNumber ? (updatedService.DUNSNumber ?? "-") : null;
            draft.HasParentCompany = updatedService.HasParentCompany != existingProvider.HasParentCompany ? updatedService.HasParentCompany : null;
            draft.ParentCompanyRegisteredName = updatedService.ParentCompanyRegisteredName != existingProvider.ParentCompanyRegisteredName ?
                (updatedService.ParentCompanyRegisteredName ?? "-") : null;
            draft.ParentCompanyLocation = updatedService.ParentCompanyLocation != existingProvider.ParentCompanyLocation ? (updatedService.ParentCompanyLocation ?? "-") : null;

          

            return draft;
        }
    }
}
