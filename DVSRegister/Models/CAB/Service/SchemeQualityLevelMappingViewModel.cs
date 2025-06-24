using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB.Service
{
    public class SchemeQualityLevelMappingViewModel : ServiceSummaryBaseViewModel
    {
        public int SchemeId { get; set; }

        [Required(ErrorMessage = "Select ‘Yes’ if the service is certified against GPG44")]
        public bool? HasGPG44 { get; set; }
        public QualityLevelViewModel QualityLevel { get; set; }
    }
}
