using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public enum ArticleStatus
    {
        New,
        Accepted,
        [Display(Name = "Minor Revision")]
        MinorRevision,
        [Display(Name = "Major Revision")]
        MajorRevision,
        Rejected
    }
}
