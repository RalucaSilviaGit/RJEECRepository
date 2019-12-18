using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class Event
    {
        public Event()
        {
            EventPhotos = new List<EventPhoto>();
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Event Name should not exceed 100 characters.")]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(100, ErrorMessage = "Event Name should not exceed 100 characters.")]
        public string Location { get; set; }
        public string Description { get; set; }
        public List<EventPhoto> EventPhotos { get; set; }
    }
}
