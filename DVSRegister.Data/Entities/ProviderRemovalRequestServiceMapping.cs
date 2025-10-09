using DVSRegister.CommonUtility.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ProviderRemovalRequestServiceMapping
    {
        public ProviderRemovalRequestServiceMapping() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ProviderRemovalRequest")]
        public int ProviderRemovalRequestId { get; set; }
        public ProviderRemovalRequest ProviderRemovalRequest { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public ServiceStatusEnum PreviousServiceStatus { get; set; }
     
    }
}
