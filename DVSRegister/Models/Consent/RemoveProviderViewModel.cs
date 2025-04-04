﻿using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class RemoveProviderViewModel
    {
        [MustBeTrue(ErrorMessage = "Select ‘Remove my details from the register of digital identity and attribute services’")]
        public bool? HasConsent { get; set; }
        public string? token { get; set; }
        public ProviderProfileDto? Provider { get; set; }
    }
}
