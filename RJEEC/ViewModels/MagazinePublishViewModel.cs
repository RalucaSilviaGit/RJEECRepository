using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class MagazinePublishViewModel
    {
        [Display(Name = "Magazine Id")]
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Volume")]
        public int Volume { get; set; }
        [Required]
        [Display(Name = "No.")]
        public int Number { get; set; }
        [Required]
        [Display(Name = "Publishing Year")]
        public int PublishingYear { get; set; }
        public List<Article> Articles { get; set; }
        [Display(Name = "Cover")]
        public IFormFile Cover { get; set; }
        public string ExistingCoverPath { get; set; }
        public bool Published { get; set; }
    }
}
