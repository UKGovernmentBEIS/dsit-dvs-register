namespace DVSRegister.Models.CAB.Service
{
    public class SchemeQualityLevelMappingViewModel 
    {
        public int SchemeId { get; set; }
        public bool? HasGpg44 { get; set; }
        public QualityLevelViewModel QualityLevel { get; set; }
    }
}
