using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly RJEECDbContext context;

        public ArticleRepository(RJEECDbContext context)
        {
            this.context = context;
        }
        public Article AddArticle(Article newArticle)
        {
            context.Articles.Add(newArticle);
            context.SaveChanges();
            return newArticle;
        }

        public Article Delete(int id)
        {
            Article deletedArticle = context.Articles.Find(id);
            if (deletedArticle != null)
            {
                context.Articles.Remove(deletedArticle);
                context.SaveChanges();
            }
            return deletedArticle;
        }

        public IEnumerable<Article> GetAllArticles()
        {
            return context.Articles;
        }

        public IEnumerable<Article> GetAllArticlesByMagazine(int id)
        {
            return context.Articles.Where(a => a.Magazine.Id == id);
        }

        public IEnumerable<Article> GetAllArticlesByPublishingYear(int id)
        {
            return context.Articles.Where(a => a.Magazine.PublishingYear == id);
        }

        public IEnumerable<Article> GetAllArticlesByStatus(int statusId)
        {
            return context.Articles.Where(a => (int)a.Status == statusId);
        }

        public IEnumerable<Article> GetAllArticlesForAuthor(int authorId)
        {
            return context.Articles.Where(a => a.contactAuthor.Id == authorId);
        }

        public Article GetArticle(int id)
        {
            return context.Articles.Find(id);
        }

        public Article Update(Article articleChanges)
        {
            var articleUpdated = context.Articles.Attach(articleChanges);
            articleUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return articleChanges;
        }
    }
}
