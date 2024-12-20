using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using RJEEC.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RJEEC.ViewModels
{
    public class ArticleDetailsViewModel
    {
        [Display(Name = "Article Number")]
        public int Id { get; set; }
        [Display(Name = "Article Title")]
        public string Title { get; set; }

        [MaxLength(5000)]
        [Display(Name = "Abstract")]
        public string Description { get; set; }

        [MaxLength(1000)]
        public string KeyWords { get; set; }

        public string Authors { get; set; }
        [Display(Name = "First Name")]
        public string AuthorFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string AuthorLastName { get; set; }
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string AuthorEmail { get; set; }
        [Display(Name = "Orcid Id")]
        public string AuthorOrcidId { get; set; }
        [Display(Name = "Researcher Id")]
        public string AuthorResearcherId { get; set; }
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

        public List<ReviewModel> ExistingReviewerDecisionFileName { get; set; } = new List<ReviewModel>();

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
