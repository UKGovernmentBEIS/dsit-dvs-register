﻿using DVSRegister.Validations;
using System.ComponentModel.DataAnnotations;

namespace DVSRegister.Models.CAB
{
	public class CertificateFileViewModel :ServiceSummaryBaseViewModel
	{
        
        [Required(ErrorMessage = "Upload the digital identity and attribute service provider's certificate of conformity")]
        [MaxFileSize(5)] // 5 MB
        [AllowedExtensions(new string[] { ".pdf" }, ErrorMessage = "The selected file must be a PDF")]        
        public IFormFile? File { get; set; }

        public string? FileName { get; set; }

        public string? FileUrl { get; set; }

        public bool? FileUploadedSuccessfully { get; set; }
        public bool FileRemoved { get; set; }


    }
}

