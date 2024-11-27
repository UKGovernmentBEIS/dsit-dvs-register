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
    [Route("consent")]
    public class ConsentController : Controller
    {
        private readonly IJwtService jwtService;
        
        private readonly IConsentService consentService;       

        public ConsentController(IJwtService jwtService, IConsentService consentService)
        {
            this.jwtService = jwtService;            
            this.consentService = consentService;          
        }


        #region Opening Loop

        [HttpGet("proceed-application-consent")]
        public async Task<ActionResult> ProceedApplicationConsent(string token)
        {
            ConsentViewModel consentViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                consentViewModel.token = token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token);
                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);
                    if (serviceDto == null || serviceDto?.ServiceStatus == ServiceStatusEnum.Received || serviceDto?.CertificateReview?.CertificateReviewStatus != CertificateReviewEnum.Approved)
                    {                       
                        return RedirectToAction("ProceedApplicationConsentError");
                    }
                    consentViewModel.Service = serviceDto;
                }
                else
                {                    
                    return RedirectToAction("ProceedApplicationConsentError");
                }
            }
            else
            {
                return RedirectToAction("ProceedApplicationConsentError");
            }
            return View(consentViewModel);
        }

        [HttpPost("proceed-application-consent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProceedApplicationGiveConsent(ConsentViewModel consentViewModel)
        {
            string email = "";
            if (!string.IsNullOrEmpty(consentViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);

                if (tokenDetails != null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByToken(tokenDetails.Token, tokenDetails.TokenId);                  
                    email = string.IsNullOrEmpty(email) ? serviceDto?.Provider.PrimaryContactEmail + ";"+ serviceDto?.Provider.SecondaryContactEmail : email;

                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await consentService.UpdateServiceStatus(serviceDto.Id, email);
                        if (genericResponse.Success)
                        {
                            await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                            return RedirectToAction("ProceedApplicationConsentSuccess");
                        }
                        else
                        {
                            return RedirectToAction("ProceedApplicationConsentError");
                        }
                    }
                    else
                    {

                        consentViewModel.Service = serviceDto;
                        return View("ProceedApplicationConsent", consentViewModel);
                    }
                }
                else
                {
                    await consentService.RemoveProceedApplicationConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                    return RedirectToAction("ProceedApplicationConsentError");
                }
            }
            else
            {
                return RedirectToAction("ProceedApplicationConsentError");
            }

        }

        [HttpGet("proceed-application-consent-success")]
        public ActionResult ProceedApplicationConsentSuccess()
        {
            return View();
        }

        [HttpGet("proceed-application-consent-error")]
        public ActionResult ProceedApplicationConsentError()
        {
            return View();
        }
        #endregion


        #region Closing the loop        

        [HttpGet("publish-service-give-consent")]
        public  async Task<ActionResult> Consent(string token)
        {
            ConsentViewModel consentViewModel = new();
            if (!string.IsNullOrEmpty(token))
            {
                consentViewModel.token =  token;
                TokenDetails tokenDetails = await jwtService.ValidateToken(token);
                if (tokenDetails!= null && tokenDetails.IsAuthorised)
                {
                    ServiceDto? serviceDto = await consentService.GetProviderAndCertificateDetailsByConsentToken(tokenDetails.Token, tokenDetails.TokenId);
                    if(serviceDto== null || serviceDto?.ServiceStatus == ServiceStatusEnum.ReadyToPublish)
                    {
                        return RedirectToAction("ConsentErrorAlreadyAgreed");
                    }
                    consentViewModel.Service = serviceDto;
                }
                else
                {
                    return RedirectToAction("ConsentErrorURLExpired");
                }
            }
            else
            {
                return RedirectToAction("ConsentError");
            }
               
           
            return View(consentViewModel);
        }

        [HttpPost("publish-service-give-consent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GiveConsent(ConsentViewModel consentViewModel)
        {
            string email = "";
            if (!string.IsNullOrEmpty(consentViewModel.token))
            {
                TokenDetails tokenDetails = await jwtService.ValidateToken(consentViewModel.token);
               
                if(tokenDetails!= null && tokenDetails.IsAuthorised)
                {
                    ServiceDto ServiceDto = await consentService.GetProviderAndCertificateDetailsByConsentToken(tokenDetails.Token, tokenDetails.TokenId);
                    email = string.IsNullOrEmpty(email) ? ServiceDto.Provider.PrimaryContactEmail + ";"+ ServiceDto.Provider.SecondaryContactEmail : email;
                    if (ModelState.IsValid)
                    {
                        GenericResponse genericResponse = await consentService.UpdateServiceAndProviderStatus(tokenDetails.Token, tokenDetails.TokenId, ServiceDto,email);
                        if (genericResponse.Success)
                        {
                            await consentService.RemoveConsentToken(tokenDetails.Token, tokenDetails.TokenId,email);
                            return RedirectToAction("ConsentSuccess");
                        }
                        else
                        {
                            return RedirectToAction("ConsentError");
                        } 
                    }
                    else
                    {
                       
                        consentViewModel.Service = ServiceDto;
                        return View("Consent", consentViewModel);
                    }
                }
                else
                {
                    await consentService.RemoveConsentToken(tokenDetails.Token, tokenDetails.TokenId, email);
                    return RedirectToAction("ConsentErrorURLExpired");
                }
            }
            else
            {
                return RedirectToAction("ConsentError");
            }         
            
        }

        [HttpGet("consent-success")]
        public ActionResult ConsentSuccess()
        {
            return View();
        }

        [HttpGet("consent-error-already-agreed")]
        public ActionResult ConsentErrorAlreadyAgreed()
        {
            return View();
        }

        [HttpGet("consent-error")]
        public ActionResult ConsentErrorError()
        {
            return View();
        }

        [HttpGet("consent-error-url-expired")]
        public ActionResult ConsentErrorURLExpired()
        {
            return View();
        }


        #endregion




    }
}
