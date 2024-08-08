namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ServiceQualityLevelMappingDto
    {      
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public int QualityLevelId { get; set; }
        public QualityLevelDto QualityLevel { get; set; }
    }
}

