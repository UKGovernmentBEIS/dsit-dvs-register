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
            if (!string.IsNullOrEmpty(token))
            {
                removeProviderViewModel.token = token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
                    if (provider == null || provider?.ProviderStatus == ProviderStatusEnum.RemovedFromRegister)
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Attempted to access removal details for an already removed provider."));
                        return RedirectToAction("RemovedProviderAlready");
                    }
                    removeProviderViewModel.Provider = provider;
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid or expired provider removal token."));
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("No token provided for provider removal details."));
                return RedirectToAction("RemoveProviderError");
            }
            return View(removeProviderViewModel);
        }



        [HttpPost("provider/provider-details")]
        public async Task<IActionResult> RemoveProviderDetails(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "Provider";

            if (!string.IsNullOrEmpty(removeProviderViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token);
                ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    if (action == "remove")
                    {
                        user = provider.PrimaryContactEmail + ";" + provider.SecondaryContactEmail;
                        if (ModelState.IsValid)
                        {
                            GenericResponse genericResponse = await removeProvider2iService.UpdateRemovalStatus(TeamEnum.Provider, tokenDetails.Token, tokenDetails.TokenId, provider, user);
                            if (genericResponse.Success)
                            {
                                await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                                return RedirectToAction("RemoveProviderSuccess");
                            }
                            else
                            {
                                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to update removal status for provider."));
                                return RedirectToAction("RemoveProviderError");
                            }
                        }
                        else
                        {
                            removeProviderViewModel.Provider = provider;
                            return View("RemoveProviderDetails", removeProviderViewModel);
                        }
                    }
                    else if (action == "cancel")
                    {
                        GenericResponse genericResponse = await removeProvider2iService.CancelRemovalRequest(TeamEnum.Provider, tokenDetails.Token, tokenDetails.TokenId, provider, user);
                        if (genericResponse.Success)
                        {
                            await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                            return RedirectToAction("RemoveProviderCancel");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to cancel removal request for provider."));
                            return RedirectToAction("RemoveProviderError");
                        }
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid action in provider removal process."));
                        return RedirectToAction("RemoveProviderError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Unauthorised provider removal attempt."));
                    await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider removal failed due to missing token."));
                return RedirectToAction("RemoveProviderError");
            }

        }

        #endregion



        #region Remove provider - Approve removal by DSIT

        [HttpGet("dsit/provider-details")]
        public async Task<ActionResult> RemoveProviderDetailsDSIT(string token)
        {
            RemoveProviderViewModel removeProviderViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                removeProviderViewModel.token = token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token, "DSIT");
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
                    if (provider == null || provider?.ProviderStatus == ProviderStatusEnum.RemovedFromRegister)
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Attempted DSIT removal for an already removed provider."));
                        return RedirectToAction("RemovedProviderAlready");
                    }
                    removeProviderViewModel.Provider = provider;
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid or expired DSIT provider removal token."));
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("No token provided for DSIT provider removal process."));
                return RedirectToAction("RemoveProviderError");
            }
            return View(removeProviderViewModel);
        }



        [HttpPost("dsit/provider-details")]
        public async Task<IActionResult> RemoveProviderDetailsDSIT(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "DSIT";

            if (!string.IsNullOrEmpty(removeProviderViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token, "DSIT");
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
                    if (action == "remove")
                    {
                        if (ModelState.IsValid)
                        {
                            GenericResponse genericResponse = await removeProvider2iService.UpdateRemovalStatus(TeamEnum.DSIT, tokenDetails.Token, tokenDetails.TokenId, provider, user);
                            if (genericResponse.Success)
                            {
                                await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                                return RedirectToAction("RemoveProviderSuccessDSIT");
                            }
                            else
                            {
                                return RedirectToAction("RemoveProviderError");
                            }
                        }
                        else
                        {
                            removeProviderViewModel.Provider = provider;
                            return View("RemoveProviderDetailsDSIT", removeProviderViewModel);
                        }
                    }
                    else if (action == "cancel")
                    {
                        GenericResponse genericResponse = await removeProvider2iService.CancelRemovalRequest(TeamEnum.DSIT, tokenDetails.Token, tokenDetails.TokenId, provider, user);
                        if(genericResponse.Success)
                        {
                            await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                            return RedirectToAction("RemoveProviderCancelDSIT");
                        }
                        else
                        {
                            return RedirectToAction("RemoveProviderError");
                        }

                    }
                    else
                    {
                        return RedirectToAction("RemoveProviderError");
                    }

                }
                else
                {
                    await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
                return RedirectToAction("RemoveProviderError");
            }

        }

        #endregion


        [HttpGet("remove-provider-success")]
        public ActionResult RemoveProviderSuccess()
        {
            return View();
        }

        [HttpGet("remove-provider-cancel")]
        public ActionResult RemoveProviderCancel()
        {
            return View();
        }

        [HttpGet("removed-provider-already")]
        public ActionResult RemovedProviderAlready()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Accessed RemovedProviderAlready page. Attempted to access removal details for an already removed provider."));
            return View();
        }

        [HttpGet("remove-provider-url-expired")]
        public ActionResult RemoveProviderURLExpired()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Accessed RemoveProviderURLExpired page. A removal request was attempted with an expired or invalid token."));
            return View();
        }

        [HttpGet("remove-provider-error")]
        public ActionResult RemoveProviderError()
        {
            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Accessed RemoveProviderError page. An unexpected error occurred in the provider removal process."));
            return View();
        }

        [HttpGet("remove-provider-success-dsit")]
        public ActionResult RemoveProviderSuccessDSIT()
        {
            return View();
        }

        [HttpGet("remove-provider-cancel-dsit")]
        public ActionResult RemoveProviderCancelDSIT()
        {
            return View();
        }

    }
}