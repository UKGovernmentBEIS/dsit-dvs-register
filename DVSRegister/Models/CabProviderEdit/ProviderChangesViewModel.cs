using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models.CAB
{
    public class ProviderChangesViewModel
    {
        public int ProviderDraftId { get; set; }
        public ProviderProfileDto? CurrentProvider { get; set; }
        public ProviderProfileDraftDto? ChangedProvider { get; set; }
        public string DSITUserEmails { get; set; }
        public Dictionary<string, List<string>>? PreviousDataKeyValuePair { get; set; }
        public Dictionary<string, List<string>>? CurrentDataKeyValuePair { get; set; }
    }
}
