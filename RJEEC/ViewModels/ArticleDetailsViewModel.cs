using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using RJEEC.Utilities;
using System.ComponentModel.DataAnnotations;

namespace RJEEC.ViewModels
{
    public class ArticleDetailsViewModel
    {
        [Display(Name = "Article number")]
        public int Id { get; set; }
        public string Title { get; set; }
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

        public string ExistingReviewerDecisionFileName { get; set; }
    }
}
