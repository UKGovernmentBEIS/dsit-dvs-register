using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{

    public class SchemeGPG45MappingDraft
    {
        public SchemeGPG45MappingDraft() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("IdentityProfile")]
        public int IdentityProfileId { get; set; }
        public IdentityProfile IdentityProfile { get; set; }
        [ForeignKey("ServiceSupSchemeMappingDraft")]
        public int ServiceSupSchemeMappingDraftId { get; set; }
        public ServiceSupSchemeMappingDraft ServiceSupSchemeMappingDraft { get; set; }
    }
}
