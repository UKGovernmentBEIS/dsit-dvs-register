using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Users;
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
        public bool IsLatestReviewVersion { get; set; }
        public int ReviewVersion { get; set; }
    }
}