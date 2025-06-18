using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB.Service
{
    public class SchemeIdentityProfileViewModel :IdentityProfileViewModel
    {
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }

    }
}
