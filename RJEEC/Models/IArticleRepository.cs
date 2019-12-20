using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IArticleRepository
    {
        IEnumerable<Article> GetAllArticles();
        IEnumerable<Article> GetAllArticlesForAuthor(int authorId);
        IEnumerable<Article> GetAllArticlesByStatus(int statusId);
        IEnumerable<Article> GetAllArticlesByMagazine(int id);
        //IEnumerable<Article> GetAllArticlesByPublishingYear(int id);
        Article GetArticle(int id);
        Article AddArticle(Article newArticle);
        Article Update(Article articleChanges);
        Article Delete(int id);
    }
}
