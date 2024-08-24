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
using System.Net.Http;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace CoolBibleVerses.Controllers
{
    public class BibleVersesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly string apiKey = "0a978d16cd48e5a4bee9e4daa1ccfcaa21a6cd5a";
        private static readonly string baseUrl = "https://api.esv.org/v3/passage/text/";

        public BibleVersesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BibleVerses
        public async Task<IActionResult> Index()
        {   
            var bibleVerses = await _context.BibleVerse
                .Include(v => v.VerseTags)
                    .ThenInclude(vt => vt.Tag)
                .Include(bb => bb.BibleBook)
                .ToListAsync();
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
                .Include(v => v.VerseTags)
                    .ThenInclude(vt => vt.Tag)
                .Include(bb => bb.BibleBook)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bibleVerse == null)
            {
                return NotFound();
            }

            return View(bibleVerse);
        }

        // GET: BibleVerses/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.BibleBooks = await _context.BibleBook.ToListAsync();
            return View();
        }

        // POST: BibleVerses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Chapter,Verse,Details")] BibleVerse bibleVerse, string? Tags, string Book)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    // Get BibleBookId from BibleBook table
                    if (Book is not null)
                    {
                        string bibleBookName = (Char.ToUpper(Book[0]) + Book.Substring(1).ToLower()).Trim();
                        var bibleBook = _context.BibleBook.Where(bb => bb.bookName == bibleBookName).FirstOrDefault();
                        if (bibleBook is null)
                        {
                            bibleVerse.BibleBookId = 0;
                        }
                        else
                        {
                            bibleVerse.BibleBookId = bibleBook.Id;
                        }
                    }

                    // Get text from ESV API and set it to the bibleVerse.Text
                    string passage = $"{Book}+{bibleVerse.Chapter}:{bibleVerse.Verse}";

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");

                    var response = await client.GetAsync($"{baseUrl}?q={Uri.EscapeDataString(passage)}&include-footnotes=false&include-headings=false&include-verse-numbers=false&include-passage-references=false&include-audio-link=false");
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(content);
                    var passages = jsonDocument.RootElement.GetProperty("passages");

                    bibleVerse.Text = passages[0].GetString();

                    _context.Add(bibleVerse);
                    await _context.SaveChangesAsync();

                    if (Tags is not null)
                    {
                        // Add tags to Tag table (adds if none exist)
                        List<int> tagIds = new List<int>();
                        string[] tagList = Tags.Split(",");
                        var dbTags = _context.Tag.ToList();

                        foreach (var tag in tagList)
                        {
                            var existingTag = dbTags.FirstOrDefault(t => t.tagText == tag.ToLower().Trim());
                            if (existingTag == null)
                            {
                                var newTag = new Tag
                                {
                                    tagText = tag.ToLower().Trim(),
                                };
                                Console.WriteLine("New Tag: " + tag);
                                _context.Tag.Add(newTag);
                                await _context.SaveChangesAsync();
                                tagIds.Add(newTag.Id);
                            }
                            else
                            {
                                tagIds.Add(existingTag.Id);
                            }

                        }

                        bibleVerse.VerseTags = new List<VerseTag>();

                        // Add tags to VerseTag table
                        foreach (var tagId in tagIds)
                        {
                            Console.WriteLine("BV ID: " + bibleVerse.Id);

                            var newVerseTag = new VerseTag
                            {
                                BibleVerseId = bibleVerse.Id,
                                TagId = tagId

                            };
                            _context.VerseTag.Add(newVerseTag);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dbexcept)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            ViewBag.BibleBooks = await _context.BibleBook.ToListAsync();
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

            var bibleVerse = await _context.BibleVerse
            .Include(bv => bv.VerseTags)
            .FirstOrDefaultAsync(m => m.Id == id);

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Book,Chapter,Verse,Details,Tags")] BibleVerse bibleVerse, string? Tags)
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

                    var curTags = _context.VerseTag.Where(vt => vt.BibleVerseId == bibleVerse.Id).ToList();
                    _context.VerseTag.RemoveRange(curTags);
                    Console.WriteLine(Tags);
                    if (!string.IsNullOrEmpty(Tags))
                    {
                        string[] tagList = Tags.Split(",");

                        // Add tags to VerseTag table
                        foreach (var tag in tagList)
                        {
                            var newTag = new Tag
                            {
                                tagText = tag.ToLower().Trim(),
                            };
                            //_context.Tag.Add(newTag);
                        }
                    }
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
                 .Include(v => v.VerseTags)
                     .ThenInclude(vt => vt.Tag)
                 .Include(bb => bb.BibleBook)
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
            var bibleVerse = await _context.BibleVerse
                .Include(v => v.VerseTags)
                    .ThenInclude(vt => vt.Tag)
                .Include(bb => bb.BibleBook)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bibleVerse != null)
            {
                _context.BibleVerse.Remove(bibleVerse);
            }
            if (bibleVerse.VerseTags != null)
            {
                _context.VerseTag.RemoveRange(bibleVerse.VerseTags);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ShowSearchResults(String SearchTerm)
        {
            string searchTerm = SearchTerm.ToLower();
            return View("Index", await _context.BibleVerse.Include(bv => bv.VerseTags).ToListAsync());
        }

        private bool BibleVerseExists(int id)
        {
            return _context.BibleVerse.Any(e => e.Id == id);
        }
    }
}
