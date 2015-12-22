using AAB.Data.DAL;
using AAB.Data.Model;
using AAB.MVC.Helpers;
using AAB.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AAB.MVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DashboardController : Controller
    {
        private AABContext db = new AABContext();
        private string BookErrorType = "Book";
        private string GenreErrorType = "Genre";

        public async Task<ActionResult> Index()
        {
            return View(await db.Books.ToListAsync());
        }

        // GET: Dashboard
        public async Task<ActionResult> Genre()
        {
            return View(await db.Genres.ToListAsync());
        }
        public async Task<ActionResult> GenreDetails(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            return View(genre);
        }
        public ActionResult GenreCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenreCreate([Bind(Include = "Id,Name")]Genre genre)
        {
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Genres.Add(genre);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Genre", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }
        public async Task<ActionResult> GenreEdit(int id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenreEdit([Bind(Include = "Id,Name")]int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            try
            {
                if (TryUpdateModel(genre))
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Genre", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }
        public async Task<ActionResult> GenreDelete([Bind(Include = "Id,Name")]int id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenreDelete([Bind(Include = "Id,Name")]int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            if (genre == null)
            {
                ViewBag.ErrorType = GenreErrorType;
                return View("NotFound");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Genres.Remove(genre);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Genre", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }

        // GET: Book
        public async Task<ActionResult> Book(string name, string searchString)
        {
            ViewBag.TitleParam = string.IsNullOrEmpty(name) || name == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.AuthorParam = string.IsNullOrEmpty(name) || name == "author_asc" ? "author_desc" : "author_asc";
            var dbBook = from b in db.Books
                         select b;
            if (!string.IsNullOrEmpty(searchString))
            {
                dbBook = dbBook.Where(s => s.Title.Contains(searchString) || s.WrittenBy.Contains(searchString) || s.NarratedBy.Contains(searchString));
            }
            switch (name)
            {
                case "title_desc":
                    dbBook = dbBook.OrderByDescending(t => t.Title);
                    break;
                case "title_asc":
                    dbBook = dbBook.OrderBy(t => t.Title);
                    break;
                case "author_desc":
                    dbBook = dbBook.OrderByDescending(t => t.WrittenBy);
                    break;
                case "author_asc":
                    dbBook = dbBook.OrderBy(t => t.WrittenBy);
                    break;
                default:
                    break;
            }
            return View(await dbBook.ToListAsync());
        }
        public ActionResult BookCreate()
        {
            var book = new Book();
            book.Genres = new List<Genre>();
            PopulateAssignedGenreData(book);
            return View();
        }

        private void PopulateAssignedGenreData(Book book)
        {
            var allGenres = db.Genres;
            var bookGenres = new HashSet<int>(book.Genres.Select(c => c.Id));
            var viewModel = new List<AssignedBookGenreData>();
            foreach (var genre in allGenres)
            {
                viewModel.Add(new AssignedBookGenreData
                {
                    GenreID = genre.Id,
                    Name = genre.Name,
                    Assigned = bookGenres.Contains(genre.Id)
                });
            }
            ViewBag.Genres = viewModel;
        }

        private void populateGenreList(object selectedGenre = null)
        {
            var genreList = from g in db.Genres
                            orderby g.Name
                            select g;
            ViewBag.GenreID = new SelectList(genreList, "GenreID", "Name", selectedGenre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookCreate([Bind(Include = "Id, Title, WrittenBy, NarratedBy, Length, Description, Rate, SampleAudio, FullAudio, CoverArt")]Book book, string[] selectedGenres)
        {
            if (book == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            try
            {
                if (selectedGenres != null)
                {
                    int? bg = null;
                    book.Genres = new List<Genre>();
                    var bookGenreToAdd = new Genre();
                    foreach (var bookgenre in selectedGenres)
                    {
                        bg = int.Parse(bookgenre);
                        var genreToAdd = await db.Genres.SingleOrDefaultAsync(i => i.Id == bg);
                        book.Genres.Add(genreToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Book", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            PopulateAssignedGenreData(book);
            return View(book);
        }

        // GET: Book/Edit
        public async Task<ActionResult> BookEdit(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            var book = new Book();
            try
            {
                book = await db.Books.Where(b => b.Id == id).SingleOrDefaultAsync();
                if (book == null)
                {
                    ViewBag.ErrorType = BookErrorType;
                    return View("NotFound");
                }
                book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();
                PopulateAssignedGenreData(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(book);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookEdit([Bind(Include = "Id, Title, WrittenBy, NarratedBy, Length, Description, Rate, SampleAudio, FullAudio, CoverArt")]int? id, string[] selectedGenres)
        {
            if (id == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            var book = new Book();
            try
            {
                book = await db.Books.Where(b => b.Id == id).SingleOrDefaultAsync();
                if (book == null)
                {
                    ViewBag.ErrorType = BookErrorType;
                    return View("NotFound");
                }
                book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();


                UpdateBookGenres(selectedGenres, book);
                if (TryUpdateModel(book))
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Book", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            PopulateAssignedGenreData(book);
            return View(book);
        }

        private void UpdateBookGenres(string[] selectedGenres, Book book)
        {
            if (selectedGenres == null)
            {
                book.Genres = new List<Genre>();
                return;
            }

            var selectedGenresHS = new HashSet<string>(selectedGenres);
            var bookGenres = new HashSet<int>
                (book.Genres.Select(c => c.Id));
            foreach (var genre in db.Genres)
            {
                if (selectedGenresHS.Contains(genre.Id.ToString()))
                {
                    if (!bookGenres.Contains(genre.Id))
                    {
                        book.Genres.Add(genre);
                    }
                }
                else
                {
                    if (bookGenres.Contains(genre.Id))
                    {
                        book.Genres.Remove(genre);
                    }
                }
            }
        }
        public async Task<ActionResult> BookDetails(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = "Book Details";
                return View("NotFound");
            }
            var book = await db.Books.SingleOrDefaultAsync(i => i.Id == id);

            if (book == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            return View(book);
        }
        public async Task<ActionResult> BookDelete(int id)
        {
            var book = await db.Books.SingleOrDefaultAsync(i => i.Id == id);
            if (book == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookDelete([Bind(Include = "Id, Title, WrittenBy, NarratedBy, Length, Description, Rate, SampleAudio, FullAudio, CoverArt")]int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorType = BookErrorType;
                return View("NotFound");
            }
            var book = new Book();
            
            try
            {
                book = await db.Books.SingleOrDefaultAsync(i => i.Id == id);
                if (book == null)
                {
                    ViewBag.ErrorType = BookErrorType;
                    return View("NotFound");
                }
                book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();
                if (ModelState.IsValid)
                {
                    db.Books.Remove(book);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return RedirectToAction("Book", "Dashboard");
        }
    }
}