using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;
        private readonly int _maxFileSizeMB;

        public MaxFileSizeAttribute(int maxFileSizeInMB)
        {
            _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
            _maxFileSizeMB = maxFileSizeInMB;

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeInBytes)
                {
                    return new ValidationResult($"The selected file must be smaller than {_maxFileSizeMB} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
