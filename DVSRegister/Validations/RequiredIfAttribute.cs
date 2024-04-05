using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string propertyName;
        private readonly object desiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            this.propertyName = propertyName;
            this.desiredValue = desiredValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(propertyName);
            if (propertyInfo == null)
                return new ValidationResult($"Unknown property: {propertyName}");

            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (propertyValue?.ToString() == desiredValue.ToString() && string.IsNullOrEmpty(value?.ToString()))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
