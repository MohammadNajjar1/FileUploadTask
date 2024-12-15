using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Objects.Models
{
        public class FileUploadModel : AModel
        {
            [Required(ErrorMessage = "Please select a file.")]
            [AllowedExtensions(new string[] { ".pdf", ".png", ".jpg", ".docx" })]
            [MaxFileSize(5 * 1024 * 1024)] // 5MB
            public IFormFile File { get; set; }

            [Required(ErrorMessage = "Please provide a description.")]
            [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
            public string Description { get; set; }
        }
}
