using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.CAB;

namespace DVSRegister.Models.CabTrustFramework
{
    public class UnderpinningServiceViewModel :ServiceSummaryBaseViewModel
    {
        public List<ServiceDto> UnderpinningServices { get; set; }
        public List<ServiceDto> ManualUnderpinningServices { get; set; }
        public string? SearchText { get; set; }
        public bool IsPublished { get; set; }
    }
}
