using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoolBibleVerses.Controllers
{
    public class BibleVerseController : Controller
    {
        // GET: BibleVerseController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BibleVerseController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BibleVerseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BibleVerseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BibleVerseController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BibleVerseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BibleVerseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BibleVerseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
