using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class SchemeGPG45MappingDto
    {
        public int Id { get; set; }
        public int IdentityProfileId { get; set; }
        public IdentityProfileDto IdentityProfile { get; set; }
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMappingDto ServiceSupSchemeMapping { get; set; }
    }
}
