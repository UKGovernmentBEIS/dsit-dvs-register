using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ProviderProfileCabMapping
    {
        public ProviderProfileCabMapping() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }

        [ForeignKey("Cab")]
        public int CabId { get; set; }
        public Cab Cab { get; set; }
    }
}
