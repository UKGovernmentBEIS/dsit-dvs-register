using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class CertificateInfoIdentityProfileMapping
    {
        public CertificateInfoIdentityProfileMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("CertificateInformation")]
        public int CertificateInformationId { get; set; }
        public CertificateInformation CertificateInformation { get; set; }

        [ForeignKey("IdentityProfile")]
        public int IdentityProfileId { get; set; }
        public IdentityProfile IdentityProfile { get; set; }
    }
}
