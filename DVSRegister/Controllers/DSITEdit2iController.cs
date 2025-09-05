using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("update-request")]
    public class DSITEdit2iController(IJwtService jwtService, IDSITEdit2iService dSITEdit2IService, IBucketService bucketService, ILogger<DSITEdit2iController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;
        private readonly IDSITEdit2iService dSITEdit2IService = dSITEdit2IService;
        private readonly IBucketService bucketService = bucketService;
        private readonly ILogger<DSITEdit2iController> _logger = logger;

        [HttpGet("provider-changes")]
        public async Task<IActionResult> ProviderChanges(string token)
        {
            ProviderReviewViewModel providerReviewViewModel = new();           
            TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
            ProviderDraftTokenDto providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidProviderUpdateRequest(tokenDetails, providerDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                providerReviewViewModel.token = tokenDetails.Token;
                providerReviewViewModel.PreviousProviderData = providerDraftTokenDto.ProviderProfileDraft.Provider;
                var (previous, current) = dSITEdit2IService.GetProviderKeyValue(providerDraftTokenDto.ProviderProfileDraft, providerDraftTokenDto.ProviderProfileDraft.Provider);
                providerReviewViewModel.PreviousDataKeyValuePair = previous;
                providerReviewViewModel.CurrentDataKeyValuePair = current;
                return View(providerReviewViewModel);

            }

            
        }



        [HttpPost("provider-changes")]
        public async Task<IActionResult> ApproveProviderChanges(ProviderReviewViewModel providerReviewViewModel, string action)
        {
            TokenDetails tokenDetails = await jwtService.ValidateToken(providerReviewViewModel.token, "DSIT");
            ProviderDraftTokenDto providerDraftTokenDto = await dSITEdit2IService.GetProviderChangesByToken(tokenDetails.Token, tokenDetails.TokenId); 
            var invalidRequestResult = await HandleInvalidProviderUpdateRequest(tokenDetails, providerDraftTokenDto);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "approve")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.UpdateProviderAndServiceStatusAndData(providerDraftTokenDto.ProviderProfileDraft);
                    if (genericResponse.Success)
                    {
                        //Token  removed when draft is deleted after update
                        return View("ProviderChangesApproved");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to update provider and service status."));
                        return View("UpdatesError");
                    }
                }
                else if (action == "cancel")
                {
                    GenericResponse genericResponse = await dSITEdit2IService.CancelProviderUpdates(providerDraftTokenDto.ProviderProfileDraft);
                    if (genericResponse.Success)
                    {
                        //Token removed when draft is deleted after cancel
                        return View("ProviderChangesCancelled");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Failed to cancel provider updates."));
                        return View("UpdatesError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("ApproveProviderChanges: Invalid action provided."));
                    return View("UpdatesError");
                }

            }
            
        }


        private async Task<ActionResult?> HandleInvalidProviderUpdateRequest(TokenDetails tokenDetails, ProviderDraftTokenDto? providerDraftTokenDto)
        {
            TokenStatusEnum tokenStatus = await dSITEdit2IService.GetEditProviderTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes: token expired"));
                return View("URLExpiredError");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes:Invalid token"));
                return View("UpdatesError");
            }
             //to do : resent logic
            else if (tokenStatus == TokenStatusEnum.RequestCompleted || tokenStatus == TokenStatusEnum.UserCancelled)
            {
                return View("UpdatesAlreadyReviewed");
            }
            else if (providerDraftTokenDto == null || providerDraftTokenDto.ProviderProfileDraft == null || providerDraftTokenDto.ProviderProfileDraft.Provider == null ||
             (providerDraftTokenDto.ProviderProfileDraft.Provider != null && providerDraftTokenDto.ProviderProfileDraft.Provider.ProviderStatus != ProviderStatusEnum.UpdatesRequested))
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider changes were already approved."));
                return View("UpdatesAlreadyReviewed");
            }
            else
            {
                return null;
            }
        }



     
    }
}