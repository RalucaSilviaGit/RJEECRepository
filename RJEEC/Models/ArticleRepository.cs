﻿using Microsoft.EntityFrameworkCore;
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
            return context.Articles.Include(a=>a.Documents).Include(a=>a.Magazine).Include(a => a.contactAuthor);
        }

        public IEnumerable<Article> GetAllArticlesByMagazine(int id)
        {
            return context.Articles.Include(a => a.Documents).Include(a => a.contactAuthor).Where(a => a.MagazineId == id);
        }

        //public IEnumerable<Article> GetAllArticlesByPublishingYear(int id)
        //{
        //    return context.Articles.Where(a => a.MagazineId == id);
        //}

        public IEnumerable<Article> GetAllArticlesByStatus(int statusId)
        {
            return context.Articles.Include(a => a.Documents).Include(a => a.contactAuthor).Where(a => (int)a.Status == statusId).OrderByDescending(a=>a.Id);
        }

        public IEnumerable<Article> GetAllArticlesForAuthor(int authorId)
        {
            return context.Articles.Include(a => a.Documents).Include(a => a.contactAuthor).Where(a => a.contactAuthorId == authorId).OrderByDescending(a => a.Id);
        }

        public Article GetArticle(int id)
        {
            return context.Articles.Include(a => a.Documents).Include(a => a.contactAuthor).FirstOrDefault(a => a.Id == id);
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
