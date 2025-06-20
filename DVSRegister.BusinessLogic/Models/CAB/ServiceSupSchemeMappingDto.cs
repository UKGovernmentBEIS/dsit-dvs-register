namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class ServiceSupSchemeMappingDto
    {     
        public int Id { get; set; }      
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }     
        public int SupplementarySchemeId { get; set; }
        public SupplementarySchemeDto SupplementaryScheme { get; set; }
        public bool? HasGpg44Mapping { get; set; }
        public ICollection<SchemeGPG44MappingDto> SchemeGpg44Mapping { get; set; }
        public ICollection<SchemeGPG45MappingDto> SchemeGpg45Mapping { get; set; }
    }
}
