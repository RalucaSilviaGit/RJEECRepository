using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IDocumentRepository
    {
        IEnumerable<Document> GetAllDocumentsForArticle(int articleId);

        Document GetDocumentByArticleAndType(int articleId, DocumentType type);
        Document GetDocument(int id);
        Document AddDocument(Document newDocument);
    }
}
