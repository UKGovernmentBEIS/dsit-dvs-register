using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using System.Text.Json.Serialization;

namespace DVSRegister.BusinessLogic.Models
{
    public class CertificateReviewDto
    {
        public int Id { get; set; }     
        public int ServiceId { get; set; }

        [JsonIgnore]
        public ServiceDto Service { get; set; }
        public int ProviProviderProfileId { get; set; }
        public ProviderProfileDto ProviderProfile { get; set; }
        public bool IsCabLogoCorrect { get; set; }
        public bool IsCabDetailsCorrect { get; set; }
        public bool IsProviderDetailsCorrect { get; set; }
        public bool IsServiceNameCorrect { get; set; }
        public bool IsRolesCertifiedCorrect { get; set; }
        public bool IsCertificationScopeCorrect { get; set; }
        public bool IsServiceSummaryCorrect { get; set; }
        public bool IsURLLinkToServiceCorrect { get; set; }
        public bool IsGPG44Correct { get; set; }
        public bool IsGPG45Correct { get; set; }
        public bool IsServiceProvisionCorrect { get; set; }
        public bool IsLocationCorrect { get; set; }
        public bool IsDateOfIssueCorrect { get; set; }
        public bool IsDateOfExpiryCorrect { get; set; }
        public bool IsAuthenticyVerifiedCorrect { get; set; }
        public string? Comments { get; set; }
        public bool? InformationMatched { get; set; }
        public string CommentsForIncorrect { get; set; }
        public string? RejectionComments { get; set; }      
        public int VerifiedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public CertificateReviewEnum CertificateReviewStatus { get; set; }
        public string? Amendments { get; set; }
    }
}
