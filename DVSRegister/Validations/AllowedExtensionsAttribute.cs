using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly HashSet<string> _extensions;
        private readonly HashSet<string> _contentTypes;

        public AllowedExtensionsAttribute(string[] extensions , string[] contentTypes)
        {
            _extensions = extensions?.Select(e => NormalizeExt(e)).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _contentTypes = contentTypes?.Select(a => a.ToLowerInvariant()).ToHashSet() ?? new HashSet<string>();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file || file.Length == 0)
                return ValidationResult.Success;


            var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();
       
            var safeName = Path.GetFileName(file.FileName ?? string.Empty);
            var ext = NormalizeExt(Path.GetExtension(safeName));

            if (string.IsNullOrEmpty(ext) || !_extensions.Contains(ext)  || !_contentTypes.Contains(contentType))
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