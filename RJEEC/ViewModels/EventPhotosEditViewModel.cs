using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.ViewModels
{
    public class EventPhotosEditViewModel
    {
        public int EventId { get; set; }
        public IEnumerable<EventPhoto> EventPhotos { get; set; }
        [Display (Name="New Photos")]
        public List<IFormFile> NewPhotos { get; set; }
    }
}
