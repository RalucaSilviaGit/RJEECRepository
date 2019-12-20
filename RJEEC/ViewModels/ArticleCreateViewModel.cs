using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RJEEC.ViewModels
{
    public class ArticleCreateViewModel
    {
        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "The field Agree Publishing Ethics must be checked.")]
        public bool AgreePublishingEthics { get; set; }
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Authors { get; set; }
        [MaxLength(1000)]
        public string KeyWords { get; set; }
        [Required]
        [MaxLength(300)]
        [Display(Name = "Abstract")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string AuthorFirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string AuthorLastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string AuthorEmail { get; set; }
        [MaxLength(20)]
        [Display(Name = "Phone")]
        public string AuthorPhone { get; set; }
        [Required]
        [Display(Name = "Upload article")]
        [DocumentValidation]
        public IFormFile ArticleContentDoc { get; set; }
        [Required]
        [Display(Name = "Publishing agreement")]
        [DocumentValidation]
        public IFormFile PulishingAgreementDoc { get; set; }
        [Display(Name = "Additional document (optional)")]
        [DocumentValidation]
        public IFormFile AdditionalDoc1 { get; set; }
        [Display(Name = "Additional document (optional)")]
        [DocumentValidation]
        public IFormFile AdditionalDoc2 { get; set; }
        public string Comments { get; set; }
    }
}
