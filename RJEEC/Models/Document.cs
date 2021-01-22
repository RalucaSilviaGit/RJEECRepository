using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public DocumentType Type { get; set; }
        public string DocumentPath { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; }
    }
}
