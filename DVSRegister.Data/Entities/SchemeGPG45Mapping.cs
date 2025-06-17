using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class SchemeGPG45Mapping
    {
        public SchemeGPG45Mapping() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("IdentityProfile")]
        public int IdentityProfileId { get; set; }
        public IdentityProfile IdentityProfile { get; set; }

        [ForeignKey("SupplementaryScheme")]
        public int SupplementarySchemeId { get; set; }
        public SupplementaryScheme SupplementaryScheme { get; set; }
    }
}
