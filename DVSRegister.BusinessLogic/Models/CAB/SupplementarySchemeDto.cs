namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class SupplementarySchemeDto
    {
        public int Id { get; set; }
        public string SchemeName { get; set; }
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersionDto TrustFrameworkVersion { get; set; }
     


       
    }
}
