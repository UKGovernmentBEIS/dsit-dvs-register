using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class CertificateInfoSupSchemeMapping
    {
        public CertificateInfoSupSchemeMapping() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("CertificateInformation")]
        public int CertificateInformationId { get; set; }
        public CertificateInformation CertificateInformation { get; set; }

        [ForeignKey("SupplementaryScheme")]
        public int SupplementarySchemeId { get; set; }
        public SupplementaryScheme SupplementaryScheme { get; set; }
    }
}
