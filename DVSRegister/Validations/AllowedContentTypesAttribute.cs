using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedContentTypesAttribute : ValidationAttribute
    {
        private readonly HashSet<string> _allowed;

        public AllowedContentTypesAttribute(string[] allowed)
        {
            _allowed = allowed?.Select(a => a.ToLowerInvariant()).ToHashSet() ?? new HashSet<string>();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
        {
            if (value is not IFormFile file || file.Length == 0)
                return ValidationResult.Success;

            var ct = (file.ContentType ?? string.Empty).ToLowerInvariant();
            return _allowed.Contains(ct) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}