using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class ServiceQualityLevelMappingDraft
    {
        public ServiceQualityLevelMappingDraft() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("ServiceDraft")]
        public int ServiceDraftId { get; set; }
        public ServiceDraft ServiceDraft { get; set; }

        [ForeignKey("QualityLevel")]
        public int QualityLevelId { get; set; }
        public QualityLevel QualityLevel { get; set; }
    }
}

