using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class IdentityProfile
    {
        public IdentityProfile() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string IdentityProfileName { get; set; }

        [ForeignKey("TrustFrameworkVersion")]
        public int TrustFrameworkVersionId { get; set; }
        public TrustFrameworkVersion TrustFrameworkVersion { get; set; }
        public IdentityProfileTypeEnum IdentityProfileType { get; set; }

    }
}
