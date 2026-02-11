using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
    public abstract class FileViewModel : ServiceSummaryBaseViewModel
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public bool? FileUploadedSuccessfully { get; set; }
        public bool FileRemoved { get; set; }
    }
    public class CertificateFileViewModel : FileViewModel
    {
        [Required(ErrorMessage = "Upload the digital identity and attribute service provider's certificate of conformity")]
        [MaxFileSize(5)]
        [AllowedExtensions(new[] { ".pdf" }, ErrorMessage = "The selected file must be a PDF")]
        [AllowedContentTypes(new[] { "application/pdf" }, ErrorMessage = "The selected file must be a PDF")]
        public IFormFile? File { get; set; }
    }
    public class TOUFileViewModel : FileViewModel
    {
        [Required(ErrorMessage = "Upload the Terms of Use for the UK CertifID trust mark")]
        [MaxFileSize(5)]
        [AllowedExtensions(new[] { ".pdf" }, ErrorMessage = "The selected file must be a PDF")]
        [AllowedContentTypes(new[] { "application/pdf" }, ErrorMessage = "The selected file must be a PDF")]
        public IFormFile? File { get; set; }
    }  
}

