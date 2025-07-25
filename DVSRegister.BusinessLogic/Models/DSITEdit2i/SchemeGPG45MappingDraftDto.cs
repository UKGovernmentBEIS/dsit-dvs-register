using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models.DSITEdit2i
{
    public class SchemeGPG45MappingDraftDto
    {      
        public int Id { get; set; }   
        public int IdentityProfileId { get; set; }
        public IdentityProfileDto IdentityProfile { get; set; }        
        public int ServiceSupSchemeMappingDraftId { get; set; }
        public ServiceSupSchemeMappingDraftDto ServiceSupSchemeMappingDraft { get; set; }
    }
}
