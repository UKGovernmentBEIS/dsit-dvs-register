using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models.DSITEdit2i
{
    public class SchemeGPG44MappingDraftDto
    {       
        public int Id { get; set; }    
        public int QualityLevelId { get; set; }
        public QualityLevelDto QualityLevel { get; set; }     
        public int ServiceSupSchemeMappingDraftId { get; set; }
        public ServiceSupSchemeMappingDraftDto ServiceSupSchemeMappingDraft { get; set; }
    }
}
