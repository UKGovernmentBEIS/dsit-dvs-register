using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Data.Entities
{
    public class PreRegistration
    {
        public PreRegistration() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsApplicationSponsor { get; set; }

        public string FullName { get; set; }
        public string JobTitle { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string Email { get; set; }

        public string TelephoneNumber { get; set; }

        public string? SponsorFullName { get; set; }
        public string? SponsorJobTitle { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? SponsorEmail { get; set; }
        public string? SponsorTelephoneNumber { get; set; }

        public ICollection<PreRegistrationCountryMapping> PreRegistrationCountryMappings { get; set; }


        [Column(TypeName = "varchar(160)")]
        public string RegisteredCompanyName { get; set; }

        public string TradingName { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string CompanyRegistrationNumber { get; set; }


        public bool HasParentCompany { get; set; }

        [Column(TypeName = "varchar(160)")]
        public string? ParentCompanyRegisteredName { get; set; }


        [Column(TypeName = "varchar(160)")]
        public string? ParentCompanyLocation { get; set; }

        public YesNoEnum ConfirmAccuracy { get; set; }
        public string? URN { get; set; }
        public PreRegistrationStatusEnum PreRegistrationStatus { get; set; }
    }
}
