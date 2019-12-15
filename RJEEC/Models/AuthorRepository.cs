using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly RJEECDbContext context;

        public AuthorRepository(RJEECDbContext context)
        {
            this.context = context;
        }

        public Author AddAuthor(Author author)
        {
            context.Authors.Add(author);
            context.SaveChanges();
            return author;
        }

        public Author Delete(int Id)
        {
            Author author = context.Authors.Find(Id);
            if (author != null)
            {
                context.Authors.Remove(author);
                context.SaveChanges();
            }
            return author;
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            return context.Authors;
        }

        public Author GetAuthor(int Id)
        {
            return context.Authors.Find(Id);
        }

        public Author Update(Author authorChanges)
        {
            var author = context.Authors.Attach(authorChanges);
            author.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return authorChanges;
        }
    }
}
