﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class ArticleReadViewModel
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public string KeyWords { get; set; }
        [Display(Name="Abstract")]
        public string Description { get; set; }
        public string Content { get; set; }
        public string Doc { get; set; }
        public string DOI { get; set; }
    }
}
