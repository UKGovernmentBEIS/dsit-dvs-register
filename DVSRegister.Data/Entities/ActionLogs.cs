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
        public int? ServiceId { get; set; } // service id can be null for provider updates
        public Service? Service { get; set; }

        [ForeignKey("User")]
        public int? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }

        [ForeignKey("User")]
        public int? UpdateRequestedUserId { get; set; }
        public User? UpdateRequestedUser { get; set; }

        [ForeignKey("User")]
        public int? UpdateApprovedUserId { get; set; }
        public User? UpdateApprovedUser { get; set; }
        public DateTime? UpdateRequestedTime { get; set; }
        public DateTime? UpdateApprovedTime { get; set; }

        [ForeignKey("CabUser")]
        public int? CabUserId { get; set; }
        public CabUser? CabUser { get; set; }     
        public string DisplayMessage { get; set; }
        public JsonDocument? OldValues {  get; set; }
        public JsonDocument? NewValues { get; set; }
        public DateTime LogDate { get; set; } // Date only
        public DateTime LoggedTime { get; set; } // Date + time
        public bool ShowInRegisterUpdates { get; set; }


        [ForeignKey("PublicInterestCheck")]
        public int? PublicInterestCheckId{ get; set; }
        public PublicInterestCheck? PublicInterestCheck { get; set; }

        [ForeignKey("CertificateReview")]
        public int? CertificateReviewId { get; set; }
        public CertificateReview? CertificateReview { get; set; }




    }
}
