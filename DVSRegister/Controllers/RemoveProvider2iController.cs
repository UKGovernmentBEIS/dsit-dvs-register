using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("remove-provider")]
    public class RemoveProvider2iController(IJwtService jwtService, IRemoveProvider2iService removeProvider2iService, ILogger<RemoveProvider2iController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;
        private readonly IRemoveProvider2iService removeProvider2iService = removeProvider2iService;
        private readonly ILogger<RemoveProvider2iController> _logger = logger;
        #region Remove provider - Approve removal by Provider 2i check

        [HttpGet("provider/provider-details")]
        public async Task<ActionResult> RemoveProviderDetails(string token)
        {
            RemoveProviderViewModel removeProviderViewModel = new();
            removeProviderViewModel.token = token;
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);          
            ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, provider);
            if (invalidRequestResult != null) 
            {
                return invalidRequestResult; 
            }
            else
            {
                removeProviderViewModel.Provider = provider;
                return View(removeProviderViewModel);
            }          
        }

      

        [HttpPost("provider/provider-details")]
        public async Task<IActionResult> RemoveProviderDetails(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "Provider";

            TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token);
            ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, provider);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "remove")
                {
                    user = provider.PrimaryContactEmail + ";" + provider.SecondaryContactEmail;
                    if (ModelState.IsValid)
                        {
                            return await HandleRemoveRequest(user,TeamEnum.Provider, tokenDetails, provider);
                        }
                        else
                        {
                            removeProviderViewModel.Provider = provider;
                            return View("RemoveProviderDetails", removeProviderViewModel);
                        }
                    }
                else if (action == "cancel")
                {
                    return await HandleCancelRequest(user,TeamEnum.Provider, tokenDetails, provider);
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid action in provider removal process."));
                    return RedirectToAction("RemoveProviderError");
                }
            }
        }

      



        #endregion



        #region Remove provider - Approve removal by DSIT

        [HttpGet("dsit/provider-details")]
        public async Task<ActionResult> RemoveProviderDetailsDSIT(string token)
        {
            RemoveProviderViewModel removeProviderViewModel = new();
            TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
            ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, provider);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                removeProviderViewModel.Provider = provider;
                return View(removeProviderViewModel);
            }           
        }



        [HttpPost("dsit/provider-details")]
        public async Task<IActionResult> RemoveProviderDetailsDSIT(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "DSIT";           
            TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token, "DSIT");
            ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, provider);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {

                if (action == "remove")
                {
                    if (ModelState.IsValid)
                    {
                        return await HandleRemoveRequest(user, TeamEnum.DSIT, tokenDetails, provider);
                    }
                    else
                    {
                        removeProviderViewModel.Provider = provider;
                        return View("RemoveProviderDetailsDSIT", removeProviderViewModel);
                    }
                }
                else if (action == "cancel")
                {
                    return await HandleCancelRequest(user, TeamEnum.DSIT, tokenDetails, provider);

                }
                else
                {
                    return RedirectToAction("RemoveProviderError");
                }
            }
           

        }

        #endregion


        


        #region Private methods

        private async Task<ActionResult?> HandleInvalidRequest(TokenDetails tokenDetails, ProviderProfileDto? provider)
        {
            TokenStatusEnum tokenStatus = await removeProvider2iService.GetTokenStatus(tokenDetails);
            if (tokenDetails.IsExpired)
            {
                await removeProvider2iService.UpdateRemovalTokenStatus(tokenDetails.ProviderProfileId, tokenDetails.ServiceIds, TokenStatusEnum.Expired);
                return View("RemoveProviderURLExpired");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid token"));
                return View("RemoveProviderError");
            }           
            else if (tokenStatus == TokenStatusEnum.AdminCancelled)
            {
                return View("RemovalRequestCancelledByDSIT");
            }
            else if (tokenStatus == TokenStatusEnum.RequestCompleted)
            {
                return View("RemovedProviderAlready");
            }
            else if (provider == null || provider?.ProviderStatus == ProviderStatusEnum.RemovedFromRegister)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider null for the given token or token already removed"));// need to keep this condition for old requests to work, can be removed later
                return View("RemovedProviderAlready");
            }           
            else
            {
                return null;
            }
        }

        private async Task<IActionResult> HandleRemoveRequest(string user, TeamEnum team, TokenDetails tokenDetails, ProviderProfileDto? provider)
        {
            GenericResponse genericResponse = await removeProvider2iService.UpdateRemovalStatus(team, tokenDetails.Token, tokenDetails.TokenId, provider, user);
            if (genericResponse.Success)
            {
                await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                return team == TeamEnum.Provider?  View("RemoveProviderSuccess"):  View("RemoveProviderSuccessDSIT");
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to update removal status for provider."));
                return View("RemoveProviderError");
            }
        }

        private async Task<IActionResult> HandleCancelRequest(string user, TeamEnum team, TokenDetails tokenDetails, ProviderProfileDto? provider)
        {
            GenericResponse genericResponse = await removeProvider2iService.CancelRemovalRequest(team, tokenDetails.Token, tokenDetails.TokenId, provider, user);
            if (genericResponse.Success)
            {
                await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                return View("RemovalRequestCancelled");
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to cancel removal request for provider."));
                return View("RemoveProviderError");
            }
        }
        #endregion

    }
}