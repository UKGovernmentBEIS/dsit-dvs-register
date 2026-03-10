using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceBulkRemovalRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Comment { get; set; }
        public bool IsRequestPending { get; set; }
        public DateTime RequestedTime { get; set; }
        public required string RequestedBy { get; set; }
        public ICollection<ServiceRemovalRequest> ServiceRemovalRequests { get; set; } = [];

    }
}
