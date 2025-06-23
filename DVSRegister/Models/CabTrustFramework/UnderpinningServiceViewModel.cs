using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CabTrustFramework
{
    public class UnderpinningServiceViewModel
    {
        public List<ServiceDto> Services { get; set; }
        public string? SearchText { get; set; }
        public bool InRegister { get; set; }
    }
}
