using Microsoft.AspNetCore.Mvc.Rendering;
using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class ArticlesInStatusViewModel
    {
        [Display(Name = "Choose Status")]
        public int? StatusId { get; set; }
        public List<Article> Articles { get; set; }
    }
}
