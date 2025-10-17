using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DVSRegister.Data.Entities
{
    public class ActionLogs
    {
        public ActionLogs() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ActionCategory")]
        public int ActionCategoryId { get; set; }
        public ActionCategory ActionCategory { get; set; }

        [ForeignKey("ActionDetails")]
        public int ActionDetailsId { get; set; }
        public ActionDetails ActionDetails { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User? User { get; set; }


        [ForeignKey("CabUser")]
        public int? CabUserId { get; set; }
        public CabUser? CabUser { get; set; }
        public string DisplayMessage { get; set; }
        public JsonDocument? OldValues {  get; set; }
        public JsonDocument? NewValues { get; set; }
        public DateTime LogDate { get; set; } // Date only
        public DateTime LoggedTime { get; set; } // Date + time

    }
}
