using DVSRegister.BusinessLogic.Models;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Models.UI;

namespace DVSRegister.Models
{
    public class RegisterUpdatesLogsViewModel : PaginationAndFilteringParameters
    {
        public List<GroupedResult<ActionLogsDto>>? RegisterUpdatesLog { get; set; }
        public DateTime? LastUpdated { get; set; }    
    }
}
