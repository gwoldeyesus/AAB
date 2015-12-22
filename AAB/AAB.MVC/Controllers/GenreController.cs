using AAB.Data.DAL;
using AAB.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AAB.MVC.Controllers
{
    public class GenreController : Controller
    {
        private AABContext db = new AABContext();
        [AllowAnonymous]
        // GET: Genre
        public async Task<ActionResult> Index()
        {
            return View(await db.Genres.ToListAsync());
        }
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            return View(genre);
        }
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(Genre genre)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Genres.Add(genre);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            return View(genre);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(int? id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            try
            {
                if (TryUpdateModel(genre))
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            return View(genre);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            var genre = await db.Genres.SingleOrDefaultAsync(i => i.Id == id);
            try
            {
                if (ModelState.IsValid)
                {
                    db.Genres.Remove(genre);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(genre);
        }
    }
}