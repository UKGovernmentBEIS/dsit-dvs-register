using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities

{
    public class SchemeGPG44MappingDraft
    {
        public SchemeGPG44MappingDraft() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("QualityLevel")]
        public int QualityLevelId { get; set; }
        public QualityLevel QualityLevel { get; set; }

        [ForeignKey("ServiceSupSchemeMappingDraft")]
        public int ServiceSupSchemeMappingDraftId { get; set; }
        public ServiceSupSchemeMappingDraft ServiceSupSchemeMappingDraft { get; set; }
    }
}
