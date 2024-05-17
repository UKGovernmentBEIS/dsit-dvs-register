﻿using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Validations;

namespace DVSRegister.Models.CAB
{
    public class IdentityProfileViewModel
    {
        public List<IdentityProfileDto>? AvailableIdentityProfiles{ get; set; }

        [EnsureMinimumCount(ErrorMessage = "Select the identity profile(s) for the digital identity and attribute service provider")]
        public List<int>? SelectedIdentityProfileIds { get; set; }
        public List<IdentityProfileDto>? SelectedIdentityProfiles { get; set; }
        public bool FromSummaryPage { get; set; }
    }
}
