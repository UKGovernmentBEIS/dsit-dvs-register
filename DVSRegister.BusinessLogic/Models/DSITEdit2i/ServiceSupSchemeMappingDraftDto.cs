using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models.DSITEdit2i;

namespace DVSRegister.BusinessLogic.Models
{
    public class ServiceSupSchemeMappingDraftDto
    {
        
        public int Id { get; set; }
        public int ServiceDraftId { get; set; }
        public ServiceDraftDto ServiceDraft { get; set; }
        public int SupplementarySchemeId { get; set; }
        public SupplementarySchemeDto SupplementaryScheme { get; set; }

        public bool? HasGpg44Mapping { get; set; }
        public ICollection<SchemeGPG44MappingDraftDto>? SchemeGPG44MappingDraft { get; set; }
        public ICollection<SchemeGPG45MappingDraftDto>? SchemeGPG45MappingDraft { get; set; }
    }
}