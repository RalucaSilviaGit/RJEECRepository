using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class EventPhoto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event ev { get; set; }
        public string PhotoPath { get; set; }
    }
}
