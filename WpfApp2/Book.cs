using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? AuthorId { get; set; }
        public Author Author { get; set; }
        public int PublishYear { get; set; }
        public string ISBN { get; set; }
        public int? GenreId { get; set; }
        public Genre Genre { get; set; }
        public int QuantilityInStock { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; } 
            public Book()
        {
            BookGenres = new List<BookGenre>();
        }
    }
}
