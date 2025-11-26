using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.BusinessLogic.Services.Edit;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Validations;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    [Route("cab-service/provider-profile")]
    public class CabProviderEditController(IEditService editService,  IUserService userService,IActionLogService actionLogService, ILogger<CabController> logger) : BaseController(logger)
    {
        private readonly IEditService editService = editService;
        private readonly IUserService userService = userService;
        private readonly IActionLogService actionLogService = actionLogService;      



        [HttpGet("change-provider-details")]
        public async Task<IActionResult> ProfileEditSummary()
        {          
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ProviderProfileDto provider = await editService.GetProviderDetails(profileSummaryViewModel.ProviderId, CabId);
            if(provider.Services == null || provider.Services.Count == 0 || provider.Services.All(x=>x.IsInRegister == false) ) 
            {
                profileSummaryViewModel.DisableAdmin2iCheck = true;
            }
            profileSummaryViewModel.RefererURL=GetRefererURL();
            return View(profileSummaryViewModel);
        }

        [HttpPost("update-provider-info")]
        public async Task<IActionResult> UpdateProviderInfo()
        {
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ProviderProfileDto providerDto = await editService.GetProviderDetails(profileSummaryViewModel.ProviderId, CabId);
            if (providerDto.Services == null || providerDto.Services.Count == 0 || providerDto.Services.All(x => x.IsInRegister == false))
            {
                profileSummaryViewModel.DisableAdmin2iCheck = true;
            }
            profileSummaryViewModel.RefererURL = GetRefererURL();
            CabUserDto cabUserDto = await userService.GetUser(UserEmail);
             ProviderProfileDto updatedInfo = ViewModelHelper.MapViewModelToDto(profileSummaryViewModel, cabUserDto.Id, CabId);
            if (providerDto == null)
                throw new InvalidOperationException("An error occurred while saving the profile summary.");

            GenericResponse genericResponse = await editService.UpdateCompanyInfoAndPublicProviderInfo(updatedInfo, UserEmail);
            if (genericResponse.Success)
            {

                var isProviderPublishedBefore = providerDto.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                var (currentCompanyInfo, previousCompanyInfo) = editService.GetCompanyValueUpdates(updatedInfo, providerDto);
                if(currentCompanyInfo.Count>0 && previousCompanyInfo.Count>0)
                await SaveActionLogs(ActionDetailsEnum.BusinessDetailsUpdate, providerDto, currentCompanyInfo, previousCompanyInfo, isProviderPublishedBefore);

                var (currentPublicContact, previousPublicContact) = editService.GetPublicContactUpdates(updatedInfo, providerDto);
                if(currentPublicContact.Count>0 && previousPublicContact.Count>0)
                await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, providerDto, currentPublicContact, previousPublicContact, isProviderPublishedBefore);

                return RedirectToAction("ProviderDetails", "CabProvider", new { providerId = providerDto.Id});
            }
            else
            {
                throw new InvalidOperationException("Failed to save provider profile.");
            }
        }
        #region Edit primary contact

        [HttpGet("edit-primary-contact/{providerId}")]
        public async Task<IActionResult> EditPrimaryContact(int providerId)
        {
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (providerId <= 0)
                throw new ArgumentException("Failed to edit primary contact. Invalid ProviderId.");

            ProviderProfileDto providerProfileDto = await editService.GetProviderDetails(providerId, CabId);
            PrimaryContactViewModel primaryContactViewModel = new()
            {
                PrimaryContactFullName = providerProfileDto.PrimaryContactFullName,
                PrimaryContactEmail = providerProfileDto.PrimaryContactEmail,
                PrimaryContactJobTitle = providerProfileDto.PrimaryContactJobTitle,
                PrimaryContactTelephoneNumber = providerProfileDto.PrimaryContactTelephoneNumber,
                ProviderId = providerProfileDto.Id
            };

            return View(primaryContactViewModel);

        }

        [HttpPost("edit-primary-contact")]
        public async Task<IActionResult> UpdatePrimaryContact(PrimaryContactViewModel primaryContactViewModel)
        {

            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (primaryContactViewModel.ProviderId <= 0)
                throw new ArgumentException("Invalid ProviderId.");

            // Fetch the latest provider data from the database
            ProviderProfileDto previousData = await editService.GetProviderDetails(primaryContactViewModel.ProviderId, CabId);
            if (previousData == null)
                throw new InvalidOperationException("ProviderProfile not found for the given ProviderId and CabId.");


            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: primaryContactViewModel.PrimaryContactEmail,
                secondaryValue: previousData.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                    )
                );

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: primaryContactViewModel.PrimaryContactTelephoneNumber,
                secondaryValue: previousData.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                    )
                );

            if (ModelState.IsValid)
            {
                ProviderProfileDto providerProfileDto = new();
                providerProfileDto.PrimaryContactFullName = primaryContactViewModel.PrimaryContactFullName;
                providerProfileDto.PrimaryContactEmail = primaryContactViewModel.PrimaryContactEmail;
                providerProfileDto.PrimaryContactJobTitle = primaryContactViewModel.PrimaryContactJobTitle;
                providerProfileDto.PrimaryContactTelephoneNumber = primaryContactViewModel.PrimaryContactTelephoneNumber;
                providerProfileDto.Id = previousData.Id;
                GenericResponse genericResponse = await editService.UpdatePrimaryContact(providerProfileDto, UserEmail);
                if (genericResponse.Success)
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = editService.GetPrimaryContactUpdates(providerProfileDto, previousData);
                    await editService.ConfirmPrimaryContactUpdates(current, previous, UserEmail, UserEmail, previousData.RegisteredName);
                    await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, previousData, current, previous, isProviderPublishedBefore);
                    var url = Url.Action("ProviderDetails", "CabProvider", new { providerId = providerProfileDto.Id });
                    return Redirect(url + "#contact-information");

                }
                else
                {
                    throw new InvalidOperationException("Failed to update primary contact information.");

                }
            }
            else
            {
                return View("EditPrimaryContact", primaryContactViewModel);
            }
        }

        #endregion

        #region Edit secondary contact

        [HttpGet("edit-secondary-contact/{providerId}")]
        public async Task<IActionResult> EditSecondaryContact(int providerId)
        {

            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (providerId <= 0)
                throw new ArgumentException("Failed to edit secondary contact. Invalid ProviderId.");

            ProviderProfileDto providerProfileDto = await editService.GetProviderDetails(providerId, CabId);
            SecondaryContactViewModel secondaryContactViewModel = new()
            {
                SecondaryContactFullName = providerProfileDto.SecondaryContactFullName,
                SecondaryContactEmail = providerProfileDto.SecondaryContactEmail,
                SecondaryContactJobTitle = providerProfileDto.SecondaryContactJobTitle,
                SecondaryContactTelephoneNumber = providerProfileDto.SecondaryContactTelephoneNumber,
                ProviderId = providerProfileDto.Id
            };

            return View(secondaryContactViewModel);
        }

        [HttpPost("edit-secondary-contact")]
        public async Task<IActionResult> UpdateSecondaryContact(SecondaryContactViewModel secondaryContactViewModel)
        {
            if (!IsValidCabId(CabId))
                return HandleInvalidCabId(CabId);

            if (secondaryContactViewModel.ProviderId <= 0)
                throw new ArgumentException("Invalid ProviderId.");

            // Fetch the latest provider data from the database
            ProviderProfileDto previousData = await editService.GetProviderDetails(secondaryContactViewModel.ProviderId, CabId);
            if (previousData == null)
                throw new InvalidOperationException("ProviderProfile not found for the given ProviderId and CabId.");

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: previousData.PrimaryContactEmail,
                secondaryValue: secondaryContactViewModel.SecondaryContactEmail,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactEmail",
                    "SecondaryContactEmail",
                    "Email address of secondary contact cannot be the same as primary contact"
                    )
                );

            ValidationHelper.ValidateDuplicateFields(
                ModelState,
                primaryValue: previousData.PrimaryContactTelephoneNumber,
                secondaryValue: secondaryContactViewModel.SecondaryContactTelephoneNumber,
                new ValidationHelper.FieldComparisonConfig(
                    "PrimaryContactTelephoneNumber",
                    "SecondaryContactTelephoneNumber",
                    "Telephone number of secondary contact cannot be the same as primary contact"
                    )
                );

            if (ModelState.IsValid)
            {
                ProviderProfileDto providerProfileDto = new();
                providerProfileDto.SecondaryContactFullName = secondaryContactViewModel.SecondaryContactFullName;
                providerProfileDto.SecondaryContactEmail = secondaryContactViewModel.SecondaryContactEmail;
                providerProfileDto.SecondaryContactJobTitle = secondaryContactViewModel.SecondaryContactJobTitle;
                providerProfileDto.SecondaryContactTelephoneNumber = secondaryContactViewModel.SecondaryContactTelephoneNumber;
                providerProfileDto.Id = previousData.Id;
                GenericResponse genericResponse = await editService.UpdateSecondaryContact(providerProfileDto, UserEmail);
                if (genericResponse.Success)
                {
                    var isProviderPublishedBefore = previousData.Services?.Any(x => x.IsInRegister || x.ServiceStatus == ServiceStatusEnum.Removed) ?? false;
                    var (current, previous) = editService.GetSecondaryContactUpdates(providerProfileDto, previousData);
                    await editService.ConfirmSecondaryContactUpdates(current, previous, UserEmail, UserEmail, previousData.RegisteredName);
                    await SaveActionLogs(ActionDetailsEnum.ProviderContactUpdate, previousData, current, previous, isProviderPublishedBefore);
                    var url = Url.Action("ProviderDetails", "CabProvider", new { providerId = providerProfileDto.Id });
                    return Redirect(url + "#contact-information");
                }
                else
                {
                    throw new InvalidOperationException("Failed to update secondary contact information.");
                }
            }
            else
            {
                return View("EditSecondaryContact", secondaryContactViewModel);
            }
        }

        #endregion


        [HttpGet("edit-summary")]
        public async Task<IActionResult> ProviderDifference()
        {
            ProviderChangesViewModel changesViewModel = new();
            ProfileSummaryViewModel profileSummaryViewModel = GetProfileSummary();
            ProviderProfileDto currentProvider = await editService.GetProviderDetails(profileSummaryViewModel.ProviderId, CabId);          
            ProviderProfileDraftDto changedProvider = CreateDraft(currentProvider, profileSummaryViewModel);
            List<string> dsitEmails = await userService.GetDSITUserEmails();
            changesViewModel.DSITUserEmails = string.Join(",", dsitEmails);
            changesViewModel.CurrentProvider = currentProvider;
            changesViewModel.ChangedProvider = changedProvider;

            (changesViewModel.PreviousDataKeyValuePair, changesViewModel.CurrentDataKeyValuePair) = await editService.GetProviderKeyValue(changedProvider, currentProvider);

            HttpContext?.Session.Set("changedProvider", changedProvider);

            return View(changesViewModel);
        }

        [HttpPost("edit-summary")]
        public async Task<IActionResult> SaveProviderDraft(ProviderChangesViewModel providerChangesViewModel)
        {

            if (providerChangesViewModel == null || providerChangesViewModel.ChangedProvider == null)
                throw new InvalidOperationException("Provider draft submission is missing required data.");        

            GenericResponse genericResponse = await editService.SaveProviderDraft(providerChangesViewModel.ChangedProvider, UserEmail, CabId);

            if (!genericResponse.Success)
                throw new InvalidOperationException("Failed to save provider draft.");

            return RedirectToAction("InformationSubmitted", new { providerId = providerChangesViewModel.ChangedProvider.ProviderProfileId });

        }

        [HttpGet("edit-request-submitted/{providerId}")]
        public async Task<IActionResult> InformationSubmitted(int providerId)
        {
            HttpContext?.Session.Remove("ProfileSummary");
            ProviderProfileDto providerDto = await editService.GetProviderDetails(providerId, CabId);
            return View(providerDto);


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
            draft.TradingName = updatedService.TradingName != existingProvider.TradingName ? (updatedService.TradingName ?? Constants.NullFieldsDisplay) : null;
            draft.CompanyRegistrationNumber = updatedService.CompanyRegistrationNumber != existingProvider.CompanyRegistrationNumber ?
                (updatedService.CompanyRegistrationNumber ?? Constants.NullFieldsDisplay) : null;
            draft.HasRegistrationNumber = updatedService.HasRegistrationNumber != existingProvider.HasRegistrationNumber ? updatedService.HasRegistrationNumber : null;
            draft.DUNSNumber = updatedService.DUNSNumber != existingProvider.DUNSNumber ? (updatedService.DUNSNumber ?? Constants.NullFieldsDisplay) : null;
            draft.HasParentCompany = updatedService.HasParentCompany != existingProvider.HasParentCompany ? updatedService.HasParentCompany : null;
            draft.ParentCompanyRegisteredName = updatedService.ParentCompanyRegisteredName != existingProvider.ParentCompanyRegisteredName ?
                (updatedService.ParentCompanyRegisteredName ?? Constants.NullFieldsDisplay) : null;
            draft.ParentCompanyLocation = updatedService.ParentCompanyLocation != existingProvider.ParentCompanyLocation ? (updatedService.ParentCompanyLocation ?? Constants.NullFieldsDisplay) : null;
            draft.ProviderWebsiteAddress = updatedService.ProviderWebsiteAddress != existingProvider.ProviderWebsiteAddress ? updatedService.ProviderWebsiteAddress : null;
            draft.PublicContactEmail = updatedService.PublicContactEmail != existingProvider.PublicContactEmail
            ? (updatedService.PublicContactEmail ?? Constants.NullFieldsDisplay)
            : null;
            draft.ProviderTelephoneNumber = updatedService.ProviderTelephoneNumber != existingProvider.ProviderTelephoneNumber
          ? (updatedService.ProviderTelephoneNumber ?? Constants.NullFieldsDisplay)
          : null;
            draft.LinkToContactPage = updatedService.LinkToContactPage != existingProvider.LinkToContactPage
           ? (updatedService.LinkToContactPage ?? Constants.NullFieldsDisplay)
           : null;          
            return draft;
        }
        private async Task SaveActionLogs(ActionDetailsEnum actionDetailsEnum, ProviderProfileDto providerProfileDto,
          Dictionary<string, List<string>> current, Dictionary<string, List<string>> previous, bool isProviderPublishedBefore)
        {
            ActionLogsDto actionLogsDto = new()
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = actionDetailsEnum,
                LoggedInUserEmail = UserEmail,
                ProviderId = providerProfileDto.Id,
                ProviderName = providerProfileDto.RegisteredName,
                PreviousData = previous,
                UpdatedData = current,
                IsProviderPreviouslyPublished = isProviderPublishedBefore
            };
            await actionLogService.SaveActionLogs(actionLogsDto);
        }
    }
}
