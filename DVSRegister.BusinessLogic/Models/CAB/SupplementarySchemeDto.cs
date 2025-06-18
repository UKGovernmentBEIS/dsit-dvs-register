namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class SupplementarySchemeDto
    {
        public int Id { get; set; }
        public string SchemeName { get; set; }
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersionDto TrustFrameworkVersion { get; set; }
        public bool? HasGpG44Mapping { get; set; }

     

        public ICollection<SchemeGPG45MappingDto>? SchemeGPG45Mapping { get; set; }
        public ICollection<SchemeGPG44MappingDto>? SchemeGPG44Mapping { get; set; }
       
    }
}
