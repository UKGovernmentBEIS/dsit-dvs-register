using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class CertificateInfoRoleMappingDto
    {
        public int Id { get; set; }     
        public int CertificateInformationId { get; set; }  
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
