using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using System.Text.Json.Serialization;

namespace DVSRegister.BusinessLogic.Models
{
    public class PublicInterestCheckDto
    {
        public int Id { get; set; }     
        public int ServiceId { get; set; }

        [JsonIgnore]
        public ServiceDto? Service { get; set; }   
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto Provider { get; set; }
        public bool? IsCompanyHouseNumberApproved { get; set; }
        public bool? IsDirectorshipsApproved { get; set; }
        public bool? IsDirectorshipsAndRelationApproved { get; set; }
        public bool? IsTradingAddressApproved { get; set; }
        public bool? IsSanctionListApproved { get; set; }
        public bool? IsUNFCApproved { get; set; }
        public bool? IsECCheckApproved { get; set; }
        public bool? IsTARICApproved { get; set; }
        public bool? IsBannedPoliticalApproved { get; set; }
        public bool? IsProvidersWebpageApproved { get; set; }
        public PublicInterestCheckEnum PublicInterestCheckStatus { get; set; }      
        public string? RejectionReasons { get; set; }
        public string? PrimaryCheckComment { get; set; }
        public string? SecondaryCheckComment { get; set; }      
        public int PrimaryCheckUserId { get; set; }
        public UserDto PrimaryCheckUser { get; set; }
        public DateTime? PrimaryCheckTime { get; set; }       
        public int? SecondaryCheckUserId { get; set; }
        public User? SecondaryCheckUser { get; set; }
        public DateTime? SecondaryCheckTime { get; set; }
    }
}