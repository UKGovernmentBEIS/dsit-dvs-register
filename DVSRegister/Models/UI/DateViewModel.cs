namespace DVSRegister.Models
{
    public class DateViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? ValidDate { get; set; }
        public bool FromSummaryPage { get; set; }
        public bool FromDetailsPage { get; set; }
        public string? PropertyName { get; set; }       
      
    }
}
