namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersionDto TrustFrameworkVersion { get; set; }
    }
}
