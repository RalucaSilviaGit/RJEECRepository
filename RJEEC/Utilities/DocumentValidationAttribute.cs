using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RJEEC.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DocumentValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null)
            {
                var supportedTypes = new[] { ".doc", ".docx", ".pdf" };
                var fileExt = System.IO.Path.GetExtension(file.FileName);
                if (!supportedTypes.Contains(fileExt))
                {
                    return new ValidationResult("File Extension Is Invalid - Only Upload .doc/.docx/.pdf files.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
