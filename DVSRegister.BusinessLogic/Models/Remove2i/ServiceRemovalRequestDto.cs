using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVSRegister.BusinessLogic.Models.Remove2i
{
    public class ServiceRemovalRequestDto
    {
        public int Id { get; set; }

     
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public ServiceRemovalReasonEnum? ServiceRemovalReason { get; set; }
        public string? RemovalReasonByCab { get; set; }
        public string? TokenId { get; set; }
        public string? Token { get; set; }    
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
