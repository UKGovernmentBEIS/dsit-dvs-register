using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Models.UI;

namespace DVSRegister.Models.Register
{
    public class AllServicesViewModel : PaginationAndFilteringParameters
    {
        public List<ServiceDto>? Services { get; set; }
    }
}
