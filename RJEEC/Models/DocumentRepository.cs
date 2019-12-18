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

        public IEnumerable<Document> GetAllDocumentsForArticle(int articleId)
        {
            return context.Documents.Where(d => d.ArticleId == articleId);
        }

        public Document GetDocument(int id)
        {
            return context.Documents.Find(id);
        }
    }
}
