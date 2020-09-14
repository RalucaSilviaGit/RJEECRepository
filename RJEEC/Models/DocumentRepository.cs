using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly RJEECDbContext context;
        public DocumentRepository(RJEECDbContext context)
        {
            this.context = context;
        }
        public Document AddDocument(Document newDocument)
        {
            context.Documents.Add(newDocument);
            context.SaveChanges();
            return newDocument;
        }
    }
}
