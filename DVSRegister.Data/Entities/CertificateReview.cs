using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class CertificateReview 
    {
        public CertificateReview() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviProviderProfileId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }     
        public string? Comments { get; set; }
        public bool? InformationMatched { get; set; }
        public bool? CertificateValid { get; set; }
        public bool? TOUValid { get; set; }
        public string? CommentsForIncorrect { get; set; }
        public string? RejectionComments { get; set; }
        public string? Amendments { get; set; }
        [ForeignKey("User")]
        public int VerifiedUser { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public CertificateReviewEnum CertificateReviewStatus { get; set; }
        public ICollection<CertificateReviewRejectionReasonMapping>? CertificateReviewRejectionReasonMapping { get; set; }
        public bool IsLatestReviewVersion { get; set; }
        public int ReviewVersion { get; set; }
        public string? ReturningSubmissionComments { get; set; }

    }
}
