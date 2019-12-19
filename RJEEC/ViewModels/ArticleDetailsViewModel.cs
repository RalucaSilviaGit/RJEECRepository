using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public string AuthorEmail { get; set; }
        [Display(Name = "Article Status")]
        public ArticleStatus Status { get; set; }
        public Magazine Magazine { get; set; }
    }
}
