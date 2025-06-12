using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{
    [Route("cab-trust-framework")]
    public class CabTrustFrameworkController(ILogger<CabTrustFrameworkController> logger) : BaseController(logger)
    {
        private readonly ILogger<CabTrustFrameworkController> logger = logger;


        [HttpGet("select-underpinning")]
        public IActionResult SelectUnderpinning()
        {
            return View();
        }

        [HttpGet("select-cab")]
        public IActionResult SelectCabOfUnderpinningService()
        {   
            // need to get cabs from repo
            var allCabs = new List<CabDto>
            {
                new CabDto { Id = 1, CabName = "Cab A" },
                new CabDto { Id = 2, CabName = "Cab B" },
                new CabDto { Id = 3, CabName = "Cab C" }
            };
            var AllCabsViewModel = new AllCabsViewModel
            {
                Cabs = allCabs
            };

            return View(AllCabsViewModel);
        }
    }
}
