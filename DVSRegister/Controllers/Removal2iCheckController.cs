using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.Remove2i;
using DVSRegister.BusinessLogic.Remove2i;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("provider-requested-removal")]
    public class Removal2iCheckController(IJwtService jwtService, IRemoveProvider2iService removeProvider2iService,
      IActionLogService actionLogService, ILogger<Removal2iCheckController> logger):Controller
    {
        private readonly IJwtService jwtService = jwtService;        
        private readonly IRemoveProvider2iService removeProvider2iService = removeProvider2iService;
        private readonly IActionLogService actionLogService = actionLogService;
        private readonly ILogger<Removal2iCheckController> _logger = logger;

        #region Remove provider - Approve removal by Provider 2i check

        [HttpGet("provider-details")]
        public async Task<ActionResult> RemoveProviderDetails(string token)
        {
            RemoveProviderViewModel removeProviderViewModel = new();
            removeProviderViewModel.token = token;
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ProviderRemovalRequestDto? removalRequest = await removeProvider2iService.GetProviderRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
            var invalidRequestResult = HandleInvalidRequest(tokenDetails, removalRequest);
            removeProviderViewModel.Provider = removalRequest.Provider;
            if (invalidRequestResult != null) 
            {
                return invalidRequestResult; 
            }
            else
            {             
                return View(removeProviderViewModel);
            }          
        }      

        [HttpPost("provider-details")]
        public async Task<IActionResult> RemoveProviderDetails(RemoveProviderViewModel removeProviderViewModel, string action)
        {
            string user = "Provider";
            TokenDetails tokenDetails = await jwtService.ValidateToken(removeProviderViewModel.token);
            ProviderRemovalRequestDto? removalRequest = await removeProvider2iService.GetProviderRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);          
            var invalidRequestResult = HandleInvalidRequest(tokenDetails, removalRequest);
          
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                var serviceIdsToBeRemoved = removalRequest!.Provider.Services?.Select(x => x.Id).ToList()??[];
                if (action == "remove")
                {
                    user = removalRequest.Provider.PrimaryContactEmail + ";" + removalRequest.Provider.SecondaryContactEmail;
                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await removeProvider2iService.ApproveProviderRemoval(removalRequest, user);
                        if (genericResponse.Success)
                        {
                            ProviderProfileDto providerWithRemovedServices = await removeProvider2iService.GetProviderDetailsWithRemovedServices(removalRequest.Provider.Id, serviceIdsToBeRemoved);
                            await actionLogService.AddMultipleActionLogs(providerWithRemovedServices?.Services?.ToList()!,
                            ActionCategoryEnum.ActionRequests, ActionDetailsEnum.ServiceAndProviderRemoved, string.Empty);
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

        #region Service
        [HttpGet("service-details")]
        public async Task<ActionResult> RemoveServiceDetails(string token)
        {
            RemoveServiceViewModel removeServiceViewModel = new();
            removeServiceViewModel.token = token;
            TokenDetails tokenDetails = await jwtService.ValidateToken(token);
            ServiceRemovalRequestDto? removalRequest = await removeProvider2iService.GetServiceRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);           
            var invalidRequestResult = HandleInvalidRequest(tokenDetails, removalRequest);
            removeServiceViewModel.Service = removalRequest.Service;
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {                
                return View(removeServiceViewModel);
            }
        }
        [HttpPost("service-details")]
        public async Task<IActionResult> RemoveServiceDetails(RemoveServiceViewModel removeServiceViewModel, string action)
        {
            string user = "Provider";
            TokenDetails tokenDetails = await jwtService.ValidateToken(removeServiceViewModel.token);
            ServiceRemovalRequestDto? removalRequest = await removeProvider2iService.GetServiceRemovalDetailsByRemovalToken(tokenDetails.Token, tokenDetails.TokenId);
            removeServiceViewModel.Service = removalRequest.Service;
            var invalidRequestResult = HandleInvalidRequest(tokenDetails, removalRequest);
            if (invalidRequestResult != null)
            {
                return invalidRequestResult;
            }
            else
            {
                if (action == "remove")
                {
                    user = removalRequest.Service.Provider.PrimaryContactEmail + ";" + removalRequest.Service.Provider.SecondaryContactEmail;
                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await removeProvider2iService.ApproveServiceRemoval(removalRequest, user);
                        if (genericResponse.Success)
                        {
                            var serviceDto = await removeProvider2iService.GetServiceDetailsWithProvider(removalRequest.ServiceId);
                            serviceDto.ServiceRemovalRequestId = removalRequest.Id;
                            await actionLogService.AddActionLog(serviceDto, ActionCategoryEnum.ActionRequests, ActionDetailsEnum.ServiceRemoved, string.Empty);
                            return View("RemoveServiceSuccess");
                        }
                        else
                        {
                            _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to update removal status for provider."));
                            return View("RemoveServiceError");
                        }
                    }
                    else
                    {                        
                        return View("RemoveServiceDetails", removeServiceViewModel);
                    }
                }
                else if (action == "cancel")
                {
                    GenericResponse genericResponse = await removeProvider2iService.CancelServiceRemoval(removalRequest, user);
                    if (genericResponse.Success)
                    {
                        var serviceDto = await removeProvider2iService.GetServiceDetailsWithProvider(removalRequest.ServiceId);
                        serviceDto.ServiceRemovalRequestId = removalRequest.Id;
                        await actionLogService.AddActionLog(serviceDto, ActionCategoryEnum.ActionRequests, ActionDetailsEnum.ServiceRemovalRequestDeclined,string.Empty);
                        return View("RemovalRequestCancelled");
                    }
                    else
                    {
                        _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Failed to cancel removal request for provider."));
                        return View("RemoveServiceError");
                    }
                }
                else
                {
                    _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid action in provider removal process."));
                    return RedirectToAction("RemoveServiceError");
                }
            }
        }
        #endregion

        #region Private methods

        private ActionResult HandleInvalidRequest(TokenDetails tokenDetails, ProviderRemovalRequestDto? removalRequest)
        {            
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Token expired"));
                return View("URLExpired");
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
                return null!;
            }
        }

        //To Do : ask design team for error screens
        private ActionResult HandleInvalidRequest(TokenDetails tokenDetails, ServiceRemovalRequestDto? removalRequest)
        {
            if (tokenDetails.IsExpired)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Token expired"));
                return View("URLExpired");
            }
            else if (!tokenDetails.IsAuthorised)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Invalid token"));
                return View("RemoveServiceError");
            }

            else if (removalRequest == null || removalRequest.Service == null || removalRequest.Service.IsInRegister == false)
            {
                _logger.LogError("{Message}", Helper.LoggingHelper.FormatErrorMessage("Request null or already removed"));
                return View("AlreadyReviewed");
            }
            else
            {
                return null!;
            }
        }


       


        #endregion

    }
}