using DVSRegister.Models.CAB;

namespace DVSRegister.Models
{
    public class DateViewModel :ServiceSummaryBaseViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? ValidDate { get; set; }     
        public string? PropertyName { get; set; }       
      
    }
}
