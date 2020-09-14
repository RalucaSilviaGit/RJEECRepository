using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Magazine
    {
        public int Id { get; set; }
        public int Volume { get; set; }
        public int Number { get; set; }
        public int PublishingYear { get; set; }
        public List<Article> Articles { get; set; }
        public string CoverPath { get; set; }
        public string BackCoverPath { get; set; }
        public bool Published { get; set; }
    }
}
