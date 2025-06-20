using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class SchemeGPG44MappingDto
    {
        public int Id { get; set; }
        public int QualityLevelId { get; set; }
        public QualityLevelDto QualityLevel { get; set; }
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMappingDto ServiceSupSchemeMapping { get; set; }
    }
}
