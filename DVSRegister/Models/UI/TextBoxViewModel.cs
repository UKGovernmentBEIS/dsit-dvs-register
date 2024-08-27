namespace DVSRegister.Models
{
    public class TextBoxViewModel
    {
        public string PropertyName { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string ErrorMessage { get; set; }
        public string Hint { get; set; }
        public bool HasError { get; set; }
        public string Class { get; set; }
        public string TextBoxType { get; set; }
        public IFormFile? File { get; set; }
    }
}
