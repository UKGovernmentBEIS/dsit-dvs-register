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


        [ForeignKey("IdentityProfile")]
        public int IdentityProfileId { get; set; }
        public IdentityProfile IdentityProfile { get; set; }
        [ForeignKey("ServiceSupSchemeMapping")]
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMapping ServiceSupSchemeMapping { get; set; }
    }
}
