﻿using System;
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
using System.Text.RegularExpressions;
using Azure;


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
                .OrderBy(bb => bb.BibleBook.Id).ThenBy(bv => bv.Chapter).ThenBy(bv => bv.Verse)
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
        [Authorize(Policy = "RequireAdminRole")]
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
        public async Task<IActionResult> Create([Bind("Chapter,Verse,VerseEnd,Details")] BibleVerse bibleVerse, string? Tags, string Book)
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
                    bibleVerse.EnteredBy = User.Identity?.Name ?? "Unknown";
                    bibleVerse.DateEntered = DateTime.Now;
                    // Get BibleBookId from BibleBook table
                    if (Book is not null)
                    {
                        string bibleBookName = "";
                        foreach (var t in Regex.Split(Book.Trim(), @"\s+")) { bibleBookName += (Char.ToUpper(t[0]) + t.Substring(1).ToLower()).Trim() + " "; }
                        var bibleBook = _context.BibleBook.Where(bb => bb.bookName == bibleBookName.Trim()).FirstOrDefault();
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
                    string passage = "";
                    if (bibleVerse.Verse is not null && bibleVerse.VerseEnd is null || bibleVerse.VerseEnd == 0)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}:{bibleVerse.Verse}";
                    }
                    else if (bibleVerse.Verse is not null && bibleVerse.VerseEnd is not null && bibleVerse.Verse < bibleVerse.VerseEnd)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}:{bibleVerse.Verse}–{bibleVerse.VerseEnd}";
                    }
                    else if (bibleVerse.Verse is null || bibleVerse.Verse == 0 && bibleVerse.VerseEnd is null || bibleVerse.VerseEnd == 0)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}";
                    }
                    else
                    {
                        passage = $"{Book} {bibleVerse.Chapter}";
                    }

                    if (passage.Contains("Psalms"))
                    {
                        passage = passage.Replace("Psalms", "Psalm");
                    }

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");

                    var response = await client.GetAsync($"{baseUrl}?q={Uri.EscapeDataString(passage)}&include-footnotes=false&include-headings=false&include-verse-numbers=true&include-passage-references=true&include-audio-link=false");
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(content);
                    var passages = jsonDocument.RootElement.GetProperty("passages");

                    var pssg = passages[0].GetString();
                    var pssgsplit = pssg.Split("\n");
                    var bcv = pssgsplit[0];
                    bibleVerse.Text = string.Join("\n", pssgsplit.Skip(2));

                    Console.WriteLine($"-{bcv}-{passage}-");
                    if (!bcv.Equals(passage))
                    {
                        ViewBag.BibleBooks = await _context.BibleBook.ToListAsync();
                        return View(bibleVerse);
                    }

                    _context.Add(bibleVerse);
                    await _context.SaveChangesAsync();

                    if (Tags is not null)
                    {
                        // Add tags to Tag table (adds if none exist)
                        List<int> tagIds = new List<int>();
                        string[] tagList = Tags.Split(",");
                        var dbTags = _context.Tag.ToList();
                        string tText = "";  

                        foreach (var tag in tagList)
                        {
                            var existingTag = dbTags.FirstOrDefault(t => t.tagText.ToUpper() == tag.ToUpper().Trim());
                            if (existingTag == null)
                            {
                                Console.WriteLine(tag);
                                if (tag is not null && tag != " " && !tag.Equals(""))
                                {
                                    foreach (var t in Regex.Split(tag.Trim(), @"\s+")) { tText += (Char.ToUpper(t[0]) + t.Substring(1).ToLower()).Trim() + " "; }
                                    var newTag = new Tag
                                    {
                                        tagText = tText.Trim()
                                    };
                                    _context.Tag.Add(newTag);
                                    await _context.SaveChangesAsync();
                                    tagIds.Add(newTag.Id);
                                    tText = "";
                                }
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
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.BibleBooks = await _context.BibleBook.ToListAsync();

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

        // POST: BibleVerses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BibleBookId,Chapter,Verse,VerseEnd,Details,VerseTags,BibleBook")] BibleVerse bibleVerse, string? Tags, string Book)
        {
            if (id != bibleVerse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("valid");
                try
                {
                    // Get BibleBookId from BibleBook table
                    if (Book is not null)
                    {
                        string bibleBookName = "";
                        foreach (var t in Regex.Split(Book.Trim(), @"\s+")) { bibleBookName += (Char.ToUpper(t[0]) + t.Substring(1).ToLower()).Trim() + " "; }
                        var bibleBook = _context.BibleBook.Where(bb => bb.bookName == bibleBookName.Trim()).FirstOrDefault();
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
                    string passage = "";
                    if (bibleVerse.Verse is not null && bibleVerse.VerseEnd is null || bibleVerse.VerseEnd == 0)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}:{bibleVerse.Verse}";
                    }
                    else if (bibleVerse.Verse is not null && bibleVerse.VerseEnd is not null && bibleVerse.Verse < bibleVerse.VerseEnd)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}:{bibleVerse.Verse}–{bibleVerse.VerseEnd}";
                    }
                    else if (bibleVerse.Verse is null || bibleVerse.Verse == 0 && bibleVerse.VerseEnd is null || bibleVerse.VerseEnd == 0)
                    {
                        passage = $"{Book} {bibleVerse.Chapter}";
                    }
                    else
                    {
                        passage = $"{Book} {bibleVerse.Chapter}";
                    }

                    if (passage.Contains("Psalms"))
                    {
                        passage = passage.Replace("Psalms", "Psalm");
                    }

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");

                    var response = await client.GetAsync($"{baseUrl}?q={Uri.EscapeDataString(passage)}&include-footnotes=false&include-headings=false&include-verse-numbers=true&include-passage-references=true&include-audio-link=false"); 
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(content);
                    var passages = jsonDocument.RootElement.GetProperty("passages");

                    var pssg = passages[0].GetString();
                    var pssgsplit = pssg.Split("\n");
                    var bcv = pssgsplit[0];
                    bibleVerse.Text = string.Join("\n", pssgsplit.Skip(2));

                    if (!bcv.Equals(passage))
                    {
                         bibleVerse = await _context.BibleVerse
                        .Include(v => v.VerseTags)
                            .ThenInclude(vt => vt.Tag)
                        .Include(bb => bb.BibleBook)
                        .FirstOrDefaultAsync(m => m.Id == id);
                        ViewBag.BibleBooks = await _context.BibleBook.ToListAsync();
                        return View(bibleVerse);
                    }

                    var verseTags = _context.VerseTag.Where(vt => vt.BibleVerseId == bibleVerse.Id);

                    if (!verseTags.IsNullOrEmpty())
                    {
                        _context.VerseTag.RemoveRange(verseTags);
                    }                    
                    _context.Update(bibleVerse);
                    await _context.SaveChangesAsync();

                    if (Tags is not null)
                    {
                        // Add tags to Tag table (adds if none exist)
                        List<int> tagIds = new List<int>();
                        string[] tagList = Tags.Split(",");
                        var dbTags = _context.Tag.ToList();
                        string tText = "";

                        foreach (var tag in tagList)
                        {
                            var existingTag = dbTags.FirstOrDefault(t => t.tagText.ToUpper() == tag.ToUpper().Trim());
                            if (existingTag == null)
                            {
                                if (tag is not null && tag != " " && !tag.Equals(""))
                                {
                                    foreach (var t in Regex.Split(tag.Trim(), @"\s+")) { tText += (Char.ToUpper(t[0]) + t.Substring(1).ToLower()).Trim() + " "; }
                                    var newTag = new Tag
                                    {
                                        tagText = tText.Trim()
                                    };
                                    _context.Tag.Add(newTag);
                                    await _context.SaveChangesAsync();
                                    tagIds.Add(newTag.Id);
                                    tText = "";
                                }
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
                            var newVerseTag = new VerseTag
                            {
                                BibleVerseId = bibleVerse.Id,
                                TagId = tagId

                            };
                            _context.VerseTag.Add(newVerseTag);
                        }
                        await _context.SaveChangesAsync();
                    }
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
        [Authorize(Policy = "RequireAdminRole")]
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
            string searchTerm = "";
            if (SearchTerm != null)
            {
                searchTerm = SearchTerm.ToLower();
            }

            return View("Index", 
                await _context.BibleVerse
                .Where(bv => 
                SearchTerm.Contains(bv.BibleBook.bookName) && SearchTerm.Contains(bv.Chapter.ToString()) && SearchTerm.Contains(bv.Verse.ToString()) || 
                bv.VerseTags.Any(vt => (vt.Tag.tagText.ToUpper()).Contains(searchTerm.ToUpper())) || 
                SearchTerm.Contains(bv.BibleBook.bookName) || 
                bv.Text.ToLower().Contains(searchTerm) ||
                bv.Details.ToLower().Contains(searchTerm))
                .Include(bv => bv.VerseTags)
                .ThenInclude(t => t.Tag)
                .Include(bb => bb.BibleBook).OrderBy(bb => bb.BibleBook.Id).ThenBy(bv => bv.Chapter).ThenBy(bv => bv.Verse)
                .ToListAsync()
                );
        }
        private bool BibleVerseExists(int id)
        {
            return _context.BibleVerse.Any(e => e.Id == id);
        }
    }
}
