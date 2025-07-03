using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ManualUnderPinningService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? ServiceName { get; set; }
        public string? ProviderName { get; set; }
        [ForeignKey("Cab")]
        public int? CabId { get; set; }
        public Cab Cab { get; set; }

        public DateTime? CertificateExpiryDate { get; set; }


    }
}
