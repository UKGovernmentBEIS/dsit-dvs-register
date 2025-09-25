using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("provider-requested-removal")]
    public class RemoveProvider2iController(IJwtService jwtService, IRemoveProvider2iService removeProvider2iService, ILogger<RemoveProvider2iController> logger) : Controller
    {
        private readonly IJwtService jwtService = jwtService;
        private readonly IRemoveProvider2iService removeProvider2iService = removeProvider2iService;
        private readonly ILogger<RemoveProvider2iController> _logger = logger;
        #region Remove provider - Approve removal by Provider 2i check

        [HttpGet("provider-details")]
        public async Task<ActionResult> RemoveProviderDetails(string token)
        {
            RemoveProviderViewModel removeProviderViewModel = new();
            removeProviderViewModel.token = token;
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ProviderRemovalRequestDto? removalRequest = await removeProvider2iService.GetProviderRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, removalRequest);
            if (invalidRequestResult != null) 
            {
                return invalidRequestResult; 
            }
            else
            {
                removeProviderViewModel.Provider = removalRequest.Provider;
                return View(removeProviderViewModel);
            }          
        }

      

        [HttpPost("provider/provider-details")]
        public async Task<IActionResult> RemoveProviderDetails(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "Provider";

            TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token);
            ProviderRemovalRequestDto? removalRequest = await removeProvider2iService.GetProviderRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);

            var invalidRequestResult = await HandleInvalidRequest(tokenDetails, removalRequest);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "remove")
                {
                    user = removalRequest.Provider.PrimaryContactEmail + ";" + removalRequest.Provider.SecondaryContactEmail;
                    if (ModelState.IsValid)
                        {
                        GenericResponse genericResponse = await removeProvider2iService.ApproveProviderRemoval(removalRequest, user);
                        if (genericResponse.Success)
                        {
                            return View("RemoveProviderSuccess");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to update removal status for provider."));
                            return View("RemoveProviderError");
                        }
                    }
                        else
                        {
                            removeProviderViewModel.Provider = removalRequest.Provider;
                            return View("RemoveProviderDetails", removeProviderViewModel);
                        }
                    }
                else if (action == "cancel")
                {
                    GenericResponse genericResponse = await removeProvider2iService.CancelProviderRemoval(removalRequest, user);
                    if (genericResponse.Success)
                    {
                        return View("RemovalRequestCancelled");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to cancel removal request for provider."));
                        return View("RemoveProviderError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid action in provider removal process."));
                    return RedirectToAction("RemoveProviderError");
                }
            }
        }   



        #endregion



    


        


        #region Private methods

        private async Task<ActionResult?> HandleInvalidRequest(TokenDetails tokenDetails, ProviderRemovalRequestDto? removalRequest)
        {            
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Token expired"));
                return View("RemoveProviderURLExpired");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid token"));
                return View("RemoveProviderError");
            }
           
            else if (removalRequest == null || removalRequest.Provider == null || removalRequest.Provider.IsInRegister == false)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Provider null for the given token or token already removed"));// need to keep this condition for old requests to work, can be removed later
                return View("AlreadyReviewed");
            }           
            else
            {
                return null;
            }
        }

       
        #endregion

    }
}