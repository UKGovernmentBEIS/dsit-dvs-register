using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class CertificateInfoRoleMapping
    {
        public CertificateInfoRoleMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("CertificateInformation")]
        public int CertificateInformationId { get; set; }
        public CertificateInformation CertificateInformation { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
