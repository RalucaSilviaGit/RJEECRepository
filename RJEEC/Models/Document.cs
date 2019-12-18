using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Type { get; set; }
        public string DocumentPath { get; set; }
    }
}
