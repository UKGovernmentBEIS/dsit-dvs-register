using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.CabTrustFramework;
using DVSRegister.Models.TrustFramework0_4;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.Controllers
{

    /// <summary>
    /// Views specific for Trustframework 0_4
    /// </summary>
    /// <param name="logger"></param>
    [Route("trust-framework-0.4")]
    public class TrustFramework0_4Controller(ILogger<TrustFramework0_4Controller> logger) : BaseController(logger)
    {
        private readonly ILogger<TrustFramework0_4Controller> logger = logger;


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
        
        [HttpGet("select-underpinning-or-white-labelled")]
        public IActionResult UnderpinningChoice()
            => View(new UnderpinningViewModel());
        
    }
}
