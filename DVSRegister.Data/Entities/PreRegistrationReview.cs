using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Data.Entities
{
    public class PreRegistrationReview
    {
        public PreRegistrationReview() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("PreRegistration")]
        public int PreRegistrationId { get; set; }
        public PreRegistration PreRegistration { get; set; }
       
        public bool IsCountryApproved { get; set; }
        public bool IsCompanyApproved { get; set; }
        public bool IsCheckListApproved { get; set; }
        public bool IsDirectorshipsAndRelationApproved { get; set; }
        public bool IsTradingAddressApproved { get; set; }
        public bool IsSanctionListApproved { get; set; }
        public bool IsUNFCApproved { get; set; }

        public bool IsECCheckApproved { get; set; }
        public bool IsTARICApproved { get; set; }

        public bool IsBannedPoliticalApproved { get; set; }
        public bool IsProvidersWebpageApproved {get;set;}
      
        public ReviewProgressEnum ReviewProgress { get; set; }
        public string? Comment { get; set; }

        [ForeignKey("User")]
        public int PrimaryCheckUserId { get; set; }

        public User PrimaryCheckUser { get; set; }

        public DateTime? PrimaryCheckTime { get; set; }

        [ForeignKey("User")]
        public int? SecondaryCheckUserId { get; set; }

        public User? SecondaryCheckUser { get; set; }

        public DateTime? SecondaryCheckTime { get; set; }
    }
   
}
