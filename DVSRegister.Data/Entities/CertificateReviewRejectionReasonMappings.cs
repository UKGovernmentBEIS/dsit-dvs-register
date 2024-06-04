using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class CertificateReviewRejectionReasonMappings
    {
        public CertificateReviewRejectionReasonMappings() { }

        [Key]
        public int Id { get; set; }

        [ForeignKey("CetificateReview")]
        public int CetificateReviewId { get; set; }
        public CertificateReview CetificateReview { get; set; }

        [ForeignKey("CertificateReviewRejectionReason")]
        public int CertificateReviewRejectionReasonId { get; set; }
        public CertificateReviewRejectionReason CertificateReviewRejectionReason { get; set; }
    }
}
