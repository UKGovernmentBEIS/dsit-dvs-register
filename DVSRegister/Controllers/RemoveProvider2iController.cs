using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("remove-provider")]
    public class RemoveProvider2iController : Controller
    {
        private readonly IJwtService jwtService;
        private readonly IRemoveProvider2iService removeProvider2iService;

        public RemoveProvider2iController(IJwtService jwtService, IRemoveProvider2iService removeProvider2iService)
        {
            this.jwtService = jwtService;
            this.removeProvider2iService = removeProvider2iService;
        }
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
                        return RedirectToAction("RemovedProviderAlready");
                    }
                    removeProviderViewModel.Provider = provider;
                }
                else
                {
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
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
                return RedirectToAction("ConsentError");
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
                        return RedirectToAction("RemovedProviderAlready");
                    }
                    removeProviderViewModel.Provider = provider;
                }
                else
                {
                    return RedirectToAction("RemoveProviderURLExpired");
                }
            }
            else
            {
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
                                return RedirectToAction("RemoveProviderSuccess");
                            }
                            else
                            {
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
                        GenericResponse genericResponse = await removeProvider2iService.CancelRemovalRequest(TeamEnum.DSIT, tokenDetails.Token, tokenDetails.TokenId, provider, user);
                        if(genericResponse.Success)
                        {
                            await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                            return RedirectToAction("RemoveProviderCancel");
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
            return View();
        }

        [HttpGet("remove-provider-url-expired")]
        public ActionResult RemoveProviderURLExpired()
        {
            return View();
        }

        [HttpGet("remove-provider-error")]
        public ActionResult RemoveProviderError()
        {
            return View();
        }

    }
}
