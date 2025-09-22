using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.Data.Entities
{
    public class ServiceRemovalRequest
    {
        public ServiceRemovalRequest() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public ServiceRemovalReasonEnum? ServiceRemovalReason { get; set; }
        public string? RemovalReasonByCab { get; set; }
        public string? TokenId { get; set; }
        public string? Token { get; set; }

        [ForeignKey("User")]
        public int? RemovalRequestedUserId { get; set; }
        public User? RemovalRequestedUser { get; set; }
        [ForeignKey("CabUser")]
        public int? RemovalRequestedCabUserId { get; set; }
        public CabUser? RemovalRequestedCabUser { get; set; }
        public bool IsRequestPending { get; set; }
        public DateTime? RemovalRequestTime { get; set; }// if removed by cron job there will not be requested time
        public DateTime? RemovedTime { get; set; }
        public ServiceStatusEnum PreviousServiceStatus { get; set; }
    }
}
