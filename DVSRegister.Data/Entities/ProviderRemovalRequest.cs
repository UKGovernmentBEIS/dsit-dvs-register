using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ProviderRemovalRequest
    {
        public ProviderRemovalRequest() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ProviderProfile")]
        public int ProviderProfileId { get; set; }
        public ProviderProfile Provider { get; set; }
        public RemovalReasonsEnum RemovalReason { get; set; }    
        public string? TokenId { get; set; }
        public string? Token { get; set; }

        [ForeignKey("User")]
        public int? RemovalRequestedUserId { get; set; }
        public  User? RemovalRequestedUser { get; set; }
        public bool IsRequestPending { get; set; }
        public DateTime? RemovalRequestTime { get; set; }
        public DateTime? RemovedTime { get; set; }
        public ProviderStatusEnum PreviousProviderStatus { get; set; }

        [ForeignKey("User")]
        public int? RemovedByUserId { get; set; }
        public User? RemovedByUser { get; set; }
    }
}
