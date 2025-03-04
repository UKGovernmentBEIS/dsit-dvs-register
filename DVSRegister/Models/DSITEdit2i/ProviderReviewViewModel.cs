﻿using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;

namespace DVSRegister.Models
{
    public class ProviderReviewViewModel
    {
        public string? token { get; set; }
        public ProviderProfileDto? PreviousProviderData { get; set; }
       

        public Dictionary<string, List<string>>? PreviousDataKeyValuePair { get; set; }
        public Dictionary<string, List<string>>? CurrentDataKeyValuePair { get; set; }
    }
}
