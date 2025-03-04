

using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class ServiceQualityLevelMappingDraftDto
    {        
        public int Id { get; set; }
        public int ServiceDraftId { get; set; }
        public ServiceDraftDto ServiceDraft { get; set; }
        public int QualityLevelId { get; set; }
        public QualityLevelDto QualityLevel { get; set; }
    }
}