using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Author contactAuthor { get; set; }
        public List<Keyword> KeyWords { get; set; }
        public Magazine Magazine { get; set; }
    }
}
