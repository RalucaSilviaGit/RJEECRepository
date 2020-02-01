using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class GetArticlesByMagazineViewModel
    {
        public string ExistingCoverPath { get; set; }
        public List<Article> Articles { get; set; }
    }
}
