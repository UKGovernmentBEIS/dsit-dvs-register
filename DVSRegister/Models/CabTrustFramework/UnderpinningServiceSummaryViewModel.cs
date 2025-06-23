using DVSRegister.BusinessLogic.Models.CAB;
namespace DVSRegister.Models.CabTrustFramework
{
    public class UnderpinningServiceSummaryViewModel
    {
        public int? SelectedUnderPinningServiceId { get; set; }
        public bool IsUnderpinningServicePublished { get; set; }
        public string UnderPinningServiceName { get; set; }
        public string UnderPinningProviderName { get; set; }
        public CabDto SelectedCab { get; set; }
        public DateTime UnderPinningServiceExpiryDate { get; set; }
    }
}
