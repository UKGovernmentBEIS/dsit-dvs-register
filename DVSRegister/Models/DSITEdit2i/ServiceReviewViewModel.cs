using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models;

namespace DVSRegister.Models
{
    public class ServiceReviewViewModel
    {
        public string? token { get; set; }
        public ServiceDto? PreviousServiceData { get; set; }
        public ServiceDraftDto? CurrentServiceData { get; set; }

        public Dictionary<string, List<string>>? PreviousDataKeyValuePair { get; set; }
        public Dictionary<string,  List<string>>? CurrentDataKeyValuePair { get; set; }
    }
}
