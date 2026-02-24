using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Book> books { get; set; }
            public Genre()
        {
            books = new List<Book>();
        }

        public ICollection<BookGenre> BookGenres { get; set; }
    }
}
