using Microsoft.AspNetCore.Mvc.Rendering;
using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class GetPublishedMagazineViewModel
    {
        [Display(Name = "Choose Magazine")]
        public int? MagazineId { get; set; }
        public List<SelectListItem> PublishedMagazines { get; set; }

        public string ExistingCoverPath { get; set; }
        public string ExistingBackCoverPath { get; set; }
        public List<Article> Articles { get; set; }
    }
}
