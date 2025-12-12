using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class PublicInterestCheck
    {
        public PublicInterestCheck() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service PreRegistration { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }       
       
        public bool? PublicInterestChecksMet { get; set; }
        public PublicInterestCheckEnum PublicInterestCheckStatus { get; set; }
        public RejectionReasonEnum? RejectionReason { get; set; }
        public string? RejectionReasons { get; set; }
        public string? PrimaryCheckComment { get; set; }
        public string? SecondaryCheckComment { get; set; }

        [ForeignKey("User")]
        public int? PrimaryCheckUserId { get; set; }
        public User? PrimaryCheckUser { get; set; }
        public DateTime? PrimaryCheckTime { get; set; }
        [ForeignKey("User")]
        public int? SecondaryCheckUserId { get; set; }
        public User? SecondaryCheckUser { get; set; }       
        public DateTime? SecondaryCheckTime { get; set; }
        public bool IsLatestReviewVersion { get; set; }
        public int ReviewVersion { get; set; }
        public string? ReturningSubmissionComments { get; set; }

        public PICheckFailReasonEnum ? PICheckFailReason { get; set; }
        public SendBackReviewTypeEnum SendBackReviewType { get; set; }

        public DateTime? SendBackTime { get; set; }

        [ForeignKey("User")]
        public int? SentBackByUserId { get; set; }
        public User? SentBackByUser { get; set; }
    }
   
}
