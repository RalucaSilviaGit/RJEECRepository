using Microsoft.AspNetCore.Http;
using RJEEC.Models;
using System.Collections.Generic;

namespace RJEEC.ViewModels
{
    public class ArticleCreateViewModel
    {
        public bool AgreePublishingEthics { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string KeyWords { get; set; }
        public string Description { get; set; }
        public Author MyProperty { get; set; }
        public IFormFile ArticleContentDoc { get; set; }
        public IFormFile PulishingAgreementDoc { get; set; }
        public IFormFile AdditionalDoc1 { get; set; }
        public IFormFile AdditionalDoc2 { get; set; }
        public string Comments { get; set; }
    }
}
