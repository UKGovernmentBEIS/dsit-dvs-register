using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class ProviderProfileDraft
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public string? RegisteredName { get; set; }
        public string? TradingName { get; set; }
        public bool? HasRegistrationNumber {  get; set; }
        public string? CompanyRegistrationNumber { get; set; }
        public string? DUNSNumber { get; set; }
        public bool? HasParentCompany { get; set; }
        public string? ParentCompanyRegisteredName { get; set; }
        public string? ParentCompanyLocation { get; set; }
        public string? PrimaryContactFullName { get; set; }
        public string? PrimaryContactJobTitle { get; set; }
        public string? PrimaryContactEmail { get; set; }
        public string? PrimaryContactTelephoneNumber { get; set; }
        public string? SecondaryContactFullName { get; set; }
        public string? SecondaryContactJobTitle { get; set; }
        public string? SecondaryContactEmail { get; set; }
        public string? SecondaryContactTelephoneNumber { get; set; }
        public string? PublicContactEmail { get; set; }
        public string? ProviderTelephoneNumber { get; set; }
        public string? ProviderWebsiteAddress { get; set; }
        
        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }

        [ForeignKey("User")]
        public int RequestedUserId{ get; set; }
        public User User { get; set; }
        public ProviderStatusEnum PreviousProviderStatus { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
