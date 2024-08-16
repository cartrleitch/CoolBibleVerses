using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoolBibleVerses.Data;
using CoolBibleVerses.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoolBibleVerses.Controllers
{
    public class BibleVersesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BibleVersesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BibleVerses
        public async Task<IActionResult> Index()
        {   
            var bibleVerses = await _context.BibleVerse.Include(v => v.VerseTags).ToListAsync();
            return View(bibleVerses);
        }

        // GET: BibleVerses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bibleVerse = await _context.BibleVerse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bibleVerse == null)
            {
                return NotFound();
            }

            return View(bibleVerse);
        }

        // GET: BibleVerses/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BibleVerses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Book,Chapter,Verse,Details,Tags")] BibleVerse bibleVerse, string Tags)
        {
            if (ModelState.IsValid)
            {
                string[] tagList = Tags.Split(",");
                bibleVerse.Text = "This is a placeholder text for the verse."; // ESV API call to get text
                _context.Add(bibleVerse);
                await _context.SaveChangesAsync();

                foreach (var tag in tagList)
                {
                    var verseTag = new VerseTag
                    {
                        Tag = tag,
                        BibleVerseId = bibleVerse.Id
                    };
                    _context.VerseTag.Add(verseTag);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(bibleVerse);
        }

        // GET: BibleVerses/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bibleVerse = await _context.BibleVerse.FindAsync(id);
            if (bibleVerse == null)
            {
                return NotFound();
            }
            return View(bibleVerse);
        }

        // POST: BibleVerses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Book,Chapter,Verse,Text")] BibleVerse bibleVerse)
        {
            if (id != bibleVerse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bibleVerse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BibleVerseExists(bibleVerse.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bibleVerse);
        }

        // GET: BibleVerses/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bibleVerse = await _context.BibleVerse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bibleVerse == null)
            {
                return NotFound();
            }

            return View(bibleVerse);
        }

        // POST: BibleVerses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bibleVerse = await _context.BibleVerse.FindAsync(id);
            if (bibleVerse != null)
            {
                _context.BibleVerse.Remove(bibleVerse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ShowSearchResults(String SearchTerm)
        {
            return View("Index", await _context.BibleVerse.Where(v=>v.Text.Contains(SearchTerm)).ToListAsync()); ;
        }

        private bool BibleVerseExists(int id)
        {
            return _context.BibleVerse.Any(e => e.Id == id);
        }
    }
}
