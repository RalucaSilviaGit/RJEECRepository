using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using RJEEC.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RJEEC.ViewModels
{
    public class ArticleDetailsViewModel
    {
        [Display(Name = "Article number")]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        [Display(Name = "First Name")]
        public string AuthorFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string AuthorLastName { get; set; }
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string AuthorEmail { get; set; }
        [Display(Name = "Article Status")]
        public ArticleStatus Status { get; set; }
        [Display(Name = "Volume")]
        public int? MagazineVolume { get; set; }
        [Display(Name = "No")]
        public int? MagazineNumber { get; set; }
        [Display(Name = "Year")]
        public int? MagazinePublishingYear { get; set; }

        [Display(Name = "Reviewer decision")]
        [DocumentValidation]
        public IFormFile ReviewerDecision { get; set; }

        public List<string> ExistingReviewerDecisionFileName { get; set; }

        public List<Document> DocumentsForArticle { get; set; }

        [Display(Name = "Upload new version for the article")]
        [DocumentValidation]
        public IFormFile ArticleContentNewDoc { get; set; }

        [Display(Name = "Additional document (optional)")]
        [DocumentValidation]
        public IFormFile AdditionalDoc1 { get; set; }
        [Display(Name = "Additional document (optional)")]
        [DocumentValidation]
        public IFormFile AdditionalDoc2 { get; set; }
    }
}
