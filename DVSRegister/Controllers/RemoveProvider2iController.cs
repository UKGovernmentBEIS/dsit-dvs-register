﻿using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models;
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
        #region Remove provider

        [HttpGet("provider-details")]
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



        [HttpPost("proceed-removal")]
        public async Task<IActionResult> ProceedRemoval(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = string.Empty;// To Do : update based on reason in provider

            if (!string.IsNullOrEmpty(removeProviderViewModel.token))
            {

                if (action == "remove")
                {
                    TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token);
                    if (tokenDetails != null && tokenDetails.IsAuthorised)
                    {
                        ProviderProfileDto? provider = await removeProvider2iService.GetProviderAndServiceDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
                        if (ModelState.IsValid)
                        {
                            GenericResponse genericResponse = await removeProvider2iService.UpdateRemovalStatus(tokenDetails.Token, tokenDetails.TokenId, provider, user);
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
                    else
                    {
                        await removeProvider2iService.RemoveRemovalToken(tokenDetails.Token, tokenDetails.TokenId, user);
                        return RedirectToAction("RemoveProviderURLExpired");
                    }
                }

                else if (action == "cancel")
                {
                    return RedirectToAction("RemoveProviderCancel");
                }
                else
                {
                    return RedirectToAction("RemoveProviderError");
                }


            }
            else
            {
                return RedirectToAction("ConsentError");
            }

        }


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
        #endregion
    }
}
