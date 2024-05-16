using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
	public class CertificateFileViewModel
	{
        
        [Required(ErrorMessage = "The selected file is empty")]
        public IFormFile? File { get; set; }

        public string? FileName { get; set; }

        public string? FileUrl { get; set; }

        public bool? FileUploadedSuccessfully { get; set; }
        public bool FromSummaryPage { get; set; }

    }
}

