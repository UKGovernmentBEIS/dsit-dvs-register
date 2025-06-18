using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class SchemeGPG45MappingDto
    {
       
        public int Id { get; set; }       
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }       
        public int IdentityProfileId { get; set; }
        public IdentityProfileDto IdentityProfile { get; set; }
        public int SupplementarySchemeId { get; set; }
        public SupplementarySchemeDto SupplementaryScheme { get; set; }
    }
}
