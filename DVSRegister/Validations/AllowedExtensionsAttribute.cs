using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DVSRegister.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly HashSet<string> _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions?.Select(e => NormalizeExt(e)).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file || file.Length == 0)
                return ValidationResult.Success;

            var safeName = Path.GetFileName(file.FileName ?? string.Empty);
            var ext = NormalizeExt(Path.GetExtension(safeName));

            if (string.IsNullOrEmpty(ext) || !_extensions.Contains(ext))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        private static string NormalizeExt(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext)) return string.Empty;
            return ext.StartsWith(".") ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant();
        }
    }
}