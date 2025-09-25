using DVSAdmin.CommonUtility.Models.Enums;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.BusinessLogic.Remove2i
{
    public class ProviderRemovalRequestDto
    {
        public int Id { get; set; }    
        public int ProviderProfileId { get; set; }
        public ProviderProfileDto Provider { get; set; }
        public RemovalReasonsEnum RemovalReason { get; set; }
        public string? TokenId { get; set; }
        public string? Token { get; set; }      
        public int? RemovalRequestedUserId { get; set; }
        public UserDto? RemovalRequestedUser { get; set; }
        public bool IsRequestPending { get; set; }
        public DateTime RemovalRequestTime { get; set; }
        public DateTime? RemovedTime { get; set; }
        public ProviderStatusEnum PreviousProviderStatus { get; set; }
    }
}
