﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Contact
    {
        [Required]
        [MaxLength(30, ErrorMessage = "First Name should not exceed 30 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Last Name should not exceed 30 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "The email format is invalid.")]
        public string Email { get; set; }
        [Required]
        public string Message{ get; set; }
    }
}
