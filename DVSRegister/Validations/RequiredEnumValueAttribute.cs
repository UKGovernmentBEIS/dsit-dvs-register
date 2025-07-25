using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredEnumValueAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            var defaultValue = Activator.CreateInstance(value.GetType());


            if (value.Equals(defaultValue))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
