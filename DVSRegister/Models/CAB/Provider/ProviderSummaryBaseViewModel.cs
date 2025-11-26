using DVSRegister.CommonUtility.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DVSRegister.Models.CAB.Provider
{
    public class ProviderSummaryBaseViewModel
    {
        public bool FromSummaryPage { get; set; }
        public int ProviderId { get; set; }

        [ValidateNever]
        public SourcePageEnum? SourcePage { get; set; }
        public string? RefererURL { get; set; }

        [ValidateNever]
        public ProviderStatusEnum? ProviderStatus { get; set; }

        public bool IsInCompleteApplication { get; set; }
        public bool DisableAdmin2iCheck { get; set; }
    }
}
