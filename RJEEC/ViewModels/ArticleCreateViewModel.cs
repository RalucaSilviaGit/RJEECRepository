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
        public bool IsTrue => true;

        [Required]
        [Display(Name = "Agree Publishing Ethics")]
        [Compare(nameof(IsTrue), ErrorMessage = "Please agree to the Publishing Ethics")]
        public bool AgreePublishingEthics { get; set; }
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Authors { get; set; }
        [MaxLength(1000)]
        public string KeyWords { get; set; }
        [MaxLength(5000)]
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
        [DataType(DataType.EmailAddress)]
        public string AuthorEmail { get; set; }
        [MaxLength(20)]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public string AuthorPhone { get; set; }
        [MaxLength(20)]
        [Display(Name = "Orcid Id")]
        public string AuthorOrcidId { get; set; }
        [MaxLength(20)]
        [Display(Name = "Researcher Id")]
        public string AuthorResearcherId { get; set; }
        [Required]
        [Display(Name = "Upload article")]
        [DocumentValidation]
        public IFormFile ArticleContentDoc { get; set; }
        [Display(Name = "Publishing agreement")]
        [DocumentMandForResearcherValidation]
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
