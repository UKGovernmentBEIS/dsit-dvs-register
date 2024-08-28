using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class CertificateReviewRejectionReasonMapping
    {
        public CertificateReviewRejectionReasonMapping() { }

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
