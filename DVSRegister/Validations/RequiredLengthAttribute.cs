using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredLengthAttribute : ValidationAttribute
    {
        public int acceptedLength { get; }

        public RequiredLengthAttribute(int acceptedLength)
        {
            this.acceptedLength = acceptedLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string strValue = value.ToString()??string.Empty;
            if (strValue.Length != acceptedLength)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
