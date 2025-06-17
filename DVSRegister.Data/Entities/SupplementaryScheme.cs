using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class SupplementaryScheme
    {
        public SupplementaryScheme() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SchemeName { get; set; }
        public int Order { get; set; }
        [ForeignKey("TrustFrameworkVersion")]
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersion TrustFrameworkVersion { get; set; }

        public ICollection<SchemeGPG44Mapping>? SchemeGPG44Mapping { get; set; }
        public ICollection<SchemeGPG45Mapping>? SchemeGPG45Mapping { get; set; }
    }
}
