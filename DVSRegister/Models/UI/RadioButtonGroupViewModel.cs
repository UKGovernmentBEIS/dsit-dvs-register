using DVSRegister.Models.UI.Enums;

namespace DVSRegister.Models
{
    public class TwoRadioButtonGroupViewModel
    {
        public string PropertyName { get; set; }
        public bool? Value { get; set; }
        public string FieldSet { get; set; }
        public HeadingEnum Heading { get; set; }
        public string LegendStyleClass { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Label1 { get; set; }
        public string Label2 { get; set; }


    }
}
