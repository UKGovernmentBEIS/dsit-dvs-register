using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.BusinessLogic.Models
{
    public class SchemeGPG44MappingDto
    {
        
        public int Id { get; set; }           
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }     
        public int QualityLevelId { get; set; }
        public QualityLevelDto QualityLevel { get; set; }     
        public SupplementarySchemeDto SupplementaryScheme { get; set; }
    }
}
