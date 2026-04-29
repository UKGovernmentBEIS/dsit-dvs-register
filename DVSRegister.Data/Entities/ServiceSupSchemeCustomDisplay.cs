using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceSupSchemeCustomDisplay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ServiceSupSchemeMapping")]
        public int ServiceSupSchemeMappingId { get; set; }
        public ServiceSupSchemeMapping? ServiceSupSchemeMapping { get; set; }
        public bool IsSupplementarySchemeHidden  { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
