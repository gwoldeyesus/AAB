using AAB.Data.Model;
using System.Collections.Generic;
using System.Data.Entity;

namespace AAB.Data.DAL
{
    public class Initializer : DropCreateDatabaseAlways<AABContext>
    {
        protected override void Seed(AABContext context)
        {
            var genres = new List<Genre>
            {
                new Genre {Name = "Drama" },
                new Genre {Name = "Science Fiction" },
                new Genre {Name = "Action and Adventure" },
                new Genre {Name = "Romance" },
                new Genre {Name = "Mystery" },
                new Genre {Name = "Horror" },
                new Genre {Name = "Self Help" },
                new Genre {Name = "Satire" },
            };
            genres.ForEach(g => context.Genres.Add(g));
            context.SaveChanges();

            var books = new List<Book>
            {
                new Book { Title = "Maleda", NarratedBy = "Getaneh", WrittenBy="Haddis Alemayehu" },
                new Book { Title = "Askual", NarratedBy = "Gutema", WrittenBy="Hundessa" },
                new Book { Title = "Mandela", NarratedBy = "Mandela", WrittenBy="Madiba" }
            };
            books.ForEach(b => context.Books.Add(b));
            context.SaveChanges();

            //var bookgenres = new List<BookGenre>
            //{
            //    new BookGenre { BookId = 1, GenreId = 1 },
            //    new BookGenre { BookId = 1, GenreId = 2 },
            //    new BookGenre { BookId = 2, GenreId = 1 }
            //};
            //bookgenres.ForEach(bg => context.BookGenres.Add(bg));
            //context.SaveChanges();
        }
    }
}