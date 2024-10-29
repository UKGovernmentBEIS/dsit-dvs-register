using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public class ServiceSummaryViewModel
    {
        public int ProviderProfileId { get; set; }     

        [Required(ErrorMessage = "Enter the service name")]
        [MaximumLength(160, ErrorMessage = "The service name must be less than 161 characters.")]
        [AcceptedCharacters(@"^[A-Za-z0-9 &@£$€¥#.,:;-]+$", ErrorMessage = "The service name must contain only letters, numbers and accepted characters.")]
        public string? ServiceName { get; set; }

        [Required(ErrorMessage = "Enter the service website address")]
        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$", ErrorMessage = "Enter a valid website address.")]
        public string? ServiceURL { get; set; }

        [Required(ErrorMessage = "Enter the company address")]       
        public string? CompanyAddress { get; set; }
        public RoleViewModel? RoleViewModel { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG44")]
        public bool? HasGPG44 { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG45")]
        public bool? HasGPG45 { get; set; }
        public QualityLevelViewModel? QualityLevelViewModel { get; set; }
        public IdentityProfileViewModel? IdentityProfileViewModel { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against a supplementary scheme")]
        public bool? HasSupplementarySchemes { get; set; }
        public SupplementarySchemeViewModel? SupplementarySchemeViewModel { get; set; }
        public string? FileName { get; set; }
        public string? FileLink { get; set; }
        public decimal? FileSizeInKb { get; set; }
        public DateTime? ConformityIssueDate { get; set; }
        public DateTime? ConformityExpiryDate { get; set; }      
        public int CabUserId { get; set; }
        public int CabId { get; set; }
        public bool FromSummaryPage { get;set; }
    }
}
