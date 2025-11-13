using DVSRegister.CommonUtility.Models.Enums;

namespace DVSRegister.Models.CAB.Provider
{
    public class ProviderSummaryBaseViewModel
    {
        public bool FromSummaryPage { get; set; }
        public int ProviderId { get; set; }
        public SourcePageEnum? SourcePage { get; set; }
        public string? RefererURL { get; set; }
    }
}
