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
    }
}
