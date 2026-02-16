using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.JWT;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DVSRegister.Controllers
{

    [Route("download")]
    public class DownloadController(IBucketService bucketService, IConsentService consentService,IJwtService jwtService, IOptions<S3Configuration> config, ILogger<DownloadController> logger) : Controller
    {
        private readonly IBucketService bucketService = bucketService;
        private readonly IConsentService consentService = consentService;
        private readonly S3Configuration config= config.Value;
        private readonly IJwtService jwtService = jwtService;
        private readonly ILogger<DownloadController> logger= logger;

        /// <summary>
        /// Download logo from s3
        /// Passing key as parameter not recommended
        /// Before using this method set logokey in session    
        /// </summary>        
        /// <returns></returns>

        [HttpGet("trustmark-logo")]
        public async Task<ActionResult> TrustmarkLogoDownload(string id)
        {
            try
            {
                DownloadLogoTokenDto? downloadLogoTokenDto = await consentService.GetDownloadTokenFromTokenId(id);
                var invalidRequest = await ValidateRequest(downloadLogoTokenDto);

                if (invalidRequest != null) return invalidRequest;

                ServiceDto? serviceDto = await consentService.GetService(downloadLogoTokenDto.ServiceId);
                serviceDto.DownloadLogoToken = downloadLogoTokenDto;
                HttpContext.Session.Set("LogoKey", serviceDto.TrustmarkNumber.SvgLogoLink);
                HttpContext.Session.Set("TrustmarkNumber", serviceDto.TrustmarkNumber);
                return View(serviceDto);

            }
            catch (Exception ex)
            {
                logger.LogError("{Message}", ex.Message);
                return View("ServiceError");
            }


        }

     

        [HttpGet("download-logo-variants")]
        public async Task<IActionResult> DownloadLogoVariants(string fileType, int serviceId, string tokenId)
        {
            try
            {
                DownloadLogoTokenDto? downloadLogoTokenDto = await consentService.GetDownloadTokenFromTokenId(tokenId);
                var invalidRequest = await ValidateRequest(downloadLogoTokenDto);

                if (invalidRequest != null) return invalidRequest;


                ServiceDto? serviceDto = await consentService.GetService(serviceId);
                TrustmarkNumberDto trustmarkNumberDto = serviceDto.TrustmarkNumber;
                string prefix = string.Empty;
                string fileName = string.Empty;
                if (fileType == "all")
                {
                    fileType = string.Empty;
                    prefix = $"processed/{trustmarkNumberDto.TrustMarkNumber}/";
                    fileName = $"{trustmarkNumberDto.TrustMarkNumber}.zip";
                }
                else if (fileType == "png" || fileType == "svg" || fileType == "jpeg")
                {
                    prefix = $"processed/{trustmarkNumberDto.TrustMarkNumber}/{fileType}/";
                    fileName = $"{trustmarkNumberDto.TrustMarkNumber}-{fileType}.zip";
                }
                else
                    throw new Exception("Invalid file type");

                var zipStream = await bucketService.GetPrefixZipAsync(config.LogoBucketName, prefix);


                return File(zipStream, "application/zip", $"{trustmarkNumberDto.TrustMarkNumber}-{fileType}.zip");

            }
            catch (Exception ex)
            {
                logger.LogError("{Message}", ex.Message);
                return View("ServiceError");
            }
        }

        /// <summary>
        /// Download logo from s3
        /// Passing key as parameter not recommended
        /// Before using this method set logokey in session    
        /// </summary>        
        /// <returns></returns>

        [HttpGet("download-logo")]
        public async Task<IActionResult> DownloadSvgLogo()
        {
            try
            {
                string logoKey = HttpContext?.Session.Get<string>("LogoKey")!;
                HttpContext?.Session.Remove("LogoKey");
                var stream = await bucketService.DownloadFileStreamAsync(logoKey, config.LogoBucketName);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                string svgXml = await reader.ReadToEndAsync();
                return Content(svgXml, "image/svg+xml");

            }
            catch (Exception ex)
            {
                logger.LogError("{Message}", ex.Message);
                return View("ServiceError");
            }
        }

        private async Task<ActionResult> ValidateRequest(DownloadLogoTokenDto? downloadLogoTokenDto)
        {
            if (downloadLogoTokenDto == null)
            {
                return View("ServiceRemoved");
            }

            TokenDetails tokenDetails = await jwtService.ValidateToken(downloadLogoTokenDto.Token);
            if (tokenDetails.IsExpired)
                return View("URLExpired");
            if (!tokenDetails.IsAuthorised || (tokenDetails.ServiceIds != null && tokenDetails.ServiceIds[0] != downloadLogoTokenDto.ServiceId))
                return View("ServiceError");

            return null!;
        }

    }
}
