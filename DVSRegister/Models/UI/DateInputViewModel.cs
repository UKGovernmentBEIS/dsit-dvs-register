namespace DVSRegister.Models
{
    public class DateInputViewModel
    {
        public string PropertyName { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }    
        public string FieldsetLegend { get; set; }
        public string Hint { get; set; }
        public string Day { get; set; }
        public bool HasDayError { get; set; }
        public string DayError { get; set; }
        public string Month { get; set; }
        public bool HasMonthError { get; set; }
        public string MonthError { get; set; }
        public bool HasYearError { get; set; }
        public string Year { get; set; }
        public string YearError { get; set; }




    }
}
