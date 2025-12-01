using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.UI;

namespace DVSRegister.Models.Home
{
    public class PendingListViewModel : PaginationParameters
    {
        public OpenTaskCount OpenTaskCount { get; set; }
        public List<ServiceDto> PendingRequests { get; set; }
        public int UserId { get; set; }
    }
}
