using AAB.Data.DAL;
using AAB.Data.Model;
using AAB.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AAB.MVC.Controllers
{
    public class BookController : Controller
    {
        private AABContext db = new AABContext();
        [AllowAnonymous]
        // GET: Book
        public async Task<ActionResult> Index(string name, string searchString)
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
        [Authorize]
        public ActionResult Create()
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(Book book, string[] selectedGenres)
        {
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
                    return RedirectToAction("Index");
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
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            var book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();
            PopulateAssignedGenreData(book);
            return View(book);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(int? id, string[] selectedGenres)
        {
            var book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();

            try
            {
                UpdateBookGenres(selectedGenres, book);
                if (TryUpdateModel(book))
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
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
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id)
        {
            var book = await db.Books.SingleOrDefaultAsync(i => i.Id == id);

            //var viewData = new BookGenreViewData();
            //viewData.Books = db.Books
            //    .Include(i => i.BookGenres);

            return View(book);
        }
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await db.Books.SingleOrDefaultAsync(i => i.Id == id);
            return View(book);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            Book book = await db.Books
                .Include(i => i.Genres)
                .Where(i => i.Id == id)
                .SingleAsync();
            try
            {
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
            return RedirectToAction("Index");
        }
    }
}