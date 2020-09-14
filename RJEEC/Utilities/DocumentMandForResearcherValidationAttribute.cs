using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DocumentMandForResearcherValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var user = httpContextAccessor.HttpContext.User;
            if (file == null && !(user.IsInRole("Admin") || user.IsInRole("SuperAdmin")))
            {
                return new ValidationResult("Please fill in and upload the publishing agreement!");
            }
            return ValidationResult.Success;
        }
    }
}
