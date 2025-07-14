using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class ServiceSupSchemeMappingDraft
    {
        public ServiceSupSchemeMappingDraft() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ServiceDraft")]
        public int ServiceDraftId { get; set; }
        public ServiceDraft ServiceDraft { get; set; }

        [ForeignKey("SupplementaryScheme")]
        public int SupplementarySchemeId { get; set; }
        public SupplementaryScheme SupplementaryScheme { get; set; }

        public bool? HasGpg44Mapping { get; set; }

        public ICollection<SchemeGPG44MappingDraft>? SchemeGPG44MappingDraft { get; set; }

        public ICollection<SchemeGPG45MappingDraft>? SchemeGPG45MappingDraft { get; set; }
    }
}
