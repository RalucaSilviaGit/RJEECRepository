using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class MockAuthorRepository : IAuthorRepository
    {
        private List<Author> _authorList;
        public MockAuthorRepository()
        {
            _authorList = new List<Author>()
            {
                new Author() { Id = 1, FirstName = "Mary", LastName="Poppins", Email = "mary@pragimtech.com" },
                new Author() { Id = 2, FirstName = "John", LastName = "IT", Email = "john@pragimtech.com" },
                new Author() { Id = 3, FirstName = "Sam", LastName = "IT", Email = "sam@pragimtech.com" },
            };
        }

        public Author AddAuthor(Author author)
        {
            author.Id = _authorList.Max(a => a.Id) + 1;
            _authorList.Add(author);
            return author;
        }

        public Author Delete(int Id)
        {
            Author author = _authorList.FirstOrDefault(a => a.Id == Id);
            if (author != null)
            {
                _authorList.Remove(author);
            }
            return author;
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            return _authorList;
        }

        public Author GetAuthor(int id)
        {
            return _authorList.FirstOrDefault(a => a.Id == id);
        }

        public Author Update(Author authorChanges)
        {
            Author author = _authorList.FirstOrDefault(a => a.Id == authorChanges.Id);
            if (author != null)
            {
                author.FirstName = authorChanges.FirstName;
                author.LastName = authorChanges.LastName;
                author.Email = authorChanges.Email;
            }
            return author;
        }
    }
}
