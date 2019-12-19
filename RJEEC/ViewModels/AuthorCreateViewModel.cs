using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class AuthorCreateViewModel
    {
        [Required]
        [MaxLength(30, ErrorMessage = "First Name should not exceed 30 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Last Name should not exceed 30 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
    }
}
