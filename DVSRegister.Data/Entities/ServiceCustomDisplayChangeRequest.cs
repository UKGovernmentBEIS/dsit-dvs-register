using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DVSRegister.Data.Entities
{

    public class ServiceCustomDisplayChangeRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }


        //JSONB storing OLD fields
        public JsonDocument OldValue { get; set; } = default!;

        // JSONB storing NEW fields
        public JsonDocument NewValue { get; set; } = default!;

        // JSONB storing HIDDEN fields
        public JsonDocument HiddenValue { get; set; } = default!;
        public string Comments { get; set; } = string.Empty;

        [ForeignKey("User")]
        public int RequestedUserId { get; set; }
        public User? RequestedUser { get; set; }       

        //Approved/Rejected/Cancelled by user
        [ForeignKey("User")]
        public int? ModifiedUserId { get; set; }
        public User? ModifiedUser { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool IsRequestPending { get; set; }
    }
}
