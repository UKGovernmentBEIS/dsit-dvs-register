using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility;


namespace DVSRegister.Data
{
    public class PreAssessment
    {
        public PreAssessment() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column(TypeName = "varchar(160)")]     
        public string RegisteredCompanyName { get; set; }

        [Column(TypeName = "varchar(8)")]        
        public string CompanyRegistrationNumber { get; set; }
        public string DIPForeignJurisdictionID { get; set; }        
        public string SROFullName { get; set; }     
        public string SRORole { get; set; }

        [Column(TypeName = "varchar(254)")]    
        public string SROEmail { get; set; }     
        public string? SROTelephoneNumber { get; set; }
        public string? AltContactRole { get; set; }

        [Column(TypeName = "varchar(254)")]
        public string? AltContactEmail { get; set; }
        public string? AltContactTelephoneNumber { get; set; }   
        public string GeographicalAreas { get; set; }    
        public bool? IDSP { get; set; }
        public bool? ASP { get; set; }
        public bool? OSP { get; set; }
        public bool? WalletProvider { get; set; }
        public bool? Other { get; set; }
        public string? OtherRoleDescription { get; set; }
        public YesNoEnum ConfirmLegalRequirements { get; set; }   
        public YesNoEnum ConfirmAccuracy { get; set; }
        public string URN { get; set; }
        public PreAssessmentStatusEnum PreAssessmentStatus { get; set; }
    }
}
