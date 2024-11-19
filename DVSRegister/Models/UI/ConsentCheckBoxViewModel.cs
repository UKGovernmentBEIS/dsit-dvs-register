namespace DVSRegister.Models
{
    public class ConsentCheckBoxViewModel
    {
        public string PropertyName { get; set; }
        public bool? Value { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Label{ get; set; }
       
    }
}
