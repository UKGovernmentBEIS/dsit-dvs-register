using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class IdentityProfileDto
    {
        public int Id { get; set; }
        public string IdentityProfileName { get; set; }
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersionDto TrustFrameworkVersion { get; set; }
        public IdentityProfileTypeEnum IdentityProfileType { get; set; }
    }
}
