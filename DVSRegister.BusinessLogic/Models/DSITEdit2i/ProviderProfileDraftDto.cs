using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.BusinessLogic.Models
{
    public class ProviderProfileDraftDto
    {
        public int Id { get; set; }
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto Provider { get; set; }

        public DateTime? ModifiedTime { get; set; }
        public string? RegisteredName { get; set; }

        [DisplayFormat(NullDisplayText = Constants.NullFieldsDisplay, ConvertEmptyStringToNull = true)]
        public string? TradingName { get; set; }
        public bool? HasRegistrationNumber { get; set; }
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

        [DisplayFormat(NullDisplayText = Constants.NullFieldsDisplay, ConvertEmptyStringToNull = true)]
        public string? PublicContactEmail { get; set; }

        [DisplayFormat(NullDisplayText = Constants.NullFieldsDisplay, ConvertEmptyStringToNull = true)]
        public string? ProviderTelephoneNumber { get; set; }
        public string? ProviderWebsiteAddress { get; set; }
        public string? CurrentProviderStatus { get; set; }
        public int RequestedUserId { get; set; }
        public UserDto User { get; set; }
    }
}
