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

        public bool? IsCabLogoCorrect { get; set; }
        public bool? IsCabDetailsCorrect { get; set; }
        public bool? IsProviderDetailsCorrect { get; set; }
        public bool? IsServiceNameCorrect { get; set; }
        public bool? IsRolesCertifiedCorrect { get; set; }
        public bool? IsCertificationScopeCorrect { get; set; }
        public bool? IsServiceSummaryCorrect { get; set; }
        public bool? IsURLLinkToServiceCorrect { get; set; }
        public bool? IsGPG44Correct { get; set; }
        public bool? IsGPG45Correct { get; set; }
        public bool? IsServiceProvisionCorrect { get; set; }
        public bool? IsLocationCorrect { get; set; }
        public bool? IsDateOfIssueCorrect { get; set; }
        public bool? IsDateOfExpiryCorrect { get; set; }
        public bool? IsAuthenticyVerifiedCorrect { get; set; }
        public string? Comments { get; set; }
        public bool? InformationMatched { get; set; }
        public string CommentsForIncorrect { get; set; }
        public string? RejectionComments { get; set; }
        public string? Amendments { get; set; }
        [ForeignKey("User")]
        public int VerifiedUser { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public CertificateReviewEnum CertificateReviewStatus { get; set; }
        public ICollection<CertificateReviewRejectionReasonMapping>? CertificateReviewRejectionReasonMapping { get; set; }

    }
}
