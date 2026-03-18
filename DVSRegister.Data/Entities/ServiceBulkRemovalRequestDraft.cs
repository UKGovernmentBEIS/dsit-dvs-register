using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceBulkRemovalRequestDraft
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }        

        [ForeignKey("User")]
        public int UserId { get; set; }       
        public DateTime? TimeStamp { get; set; }
    }
}
