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

        public IEnumerable<Author> GetAllAuthors()
        {
            return _authorList;
        }

        public Author GetAuthor(int id)
        {
            return _authorList.FirstOrDefault(a => a.Id == id);
        }
    }
}
