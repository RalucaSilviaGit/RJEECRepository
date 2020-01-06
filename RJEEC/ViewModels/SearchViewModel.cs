using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class SearchViewModel
    {
        [Display(Name="Author name")]
        public string AuthorName { get; set; }
        public int? Volume { get; set; }
        public int? Year { get; set; }
        public string Keywords { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}
