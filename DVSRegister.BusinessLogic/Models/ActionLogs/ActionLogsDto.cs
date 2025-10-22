using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;

namespace DVSRegister.BusinessLogic.Models
{
    public class ActionLogsDto
    {
        public ActionCategoryEnum ActionCategoryEnum { get; set; }
        public ActionDetailsEnum ActionDetailsEnum { get; set; }
        public int RequestedUserId { get; set; }
        public string LoggedInUserEmail { get; set; }
        public int? ServiceId { get; set; }
        public int ProviderId { get; set; }
        public string ServiceName { get; set; }
        public string ProviderName { get; set; }
        public Dictionary<string, List<string>>? PreviousData { get; set; }
        public Dictionary<string, List<string>>? UpdatedData { get; set; }
        public DateTime? UpdateRequestedTime { get; set; }
    }
}
