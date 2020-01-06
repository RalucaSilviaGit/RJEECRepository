using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Article
    {
        public Article()
        {
            Documents = new List<Document>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int contactAuthorId { get; set; }
        public string KeyWords { get; set; }
        public string Authors { get; set; }
        public ArticleStatus Status { get; set; }
        public List<Document> Documents { get; set; }
        public int? MagazineId { get; set; }
        public Magazine Magazine { get; set; }
        public bool AgreePublishingEthics { get; set; }
        public string Comments { get; set; }
    }
}
