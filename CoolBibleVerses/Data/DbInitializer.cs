using CoolBibleVerses.Models;
using System;
using System.Linq;

namespace CoolBibleVerses.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any BibleVerses.
            if (context.BibleBook.Any())
            {
                return;   // DB has been seeded
            }

            var bibleBooks = new BibleBook[]
            {
                new BibleBook {bookName = "Genesis"},
                new BibleBook {bookName = "Exodus"},
                new BibleBook {bookName = "Leviticus"},
                new BibleBook {bookName = "Numbers"},
                new BibleBook {bookName = "Deuteronomy"},
                new BibleBook {bookName = "Joshua"},
                new BibleBook {bookName = "Judges"},
                new BibleBook {bookName = "Ruth"},
                new BibleBook {bookName = "1 Samuel"},
                new BibleBook {bookName = "2 Samuel"},
                new BibleBook {bookName = "1 Kings"},
                new BibleBook {bookName = "2 Kings"},
                new BibleBook {bookName = "1 Chronicles"},
                new BibleBook {bookName = "2 Chronicles"},
                new BibleBook {bookName = "Ezra"},
                new BibleBook {bookName = "Nehemiah"},
                new BibleBook {bookName = "Esther"},
                new BibleBook {bookName = "Job"},
                new BibleBook {bookName = "Psalms"},
                new BibleBook {bookName = "Proverbs"},
                new BibleBook {bookName = "Ecclesiastes"},
                new BibleBook {bookName = "Song of Solomon"},
                new BibleBook {bookName = "Isaiah"},
                new BibleBook {bookName = "Jeremiah"},
                new BibleBook {bookName = "Lamentations"},
                new BibleBook {bookName = "Ezekiel"},
                new BibleBook {bookName = "Daniel"},
                new BibleBook {bookName = "Hosea"},
                new BibleBook {bookName = "Joel"},
                new BibleBook {bookName = "Amos"},
                new BibleBook {bookName = "Obadiah"},
                new BibleBook {bookName = "Jonah"},
                new BibleBook {bookName = "Micah"},
                new BibleBook {bookName = "Nahum"},
                new BibleBook {bookName = "Habakkuk"},
                new BibleBook {bookName = "Zephaniah"},
                new BibleBook {bookName = "Haggai"},
                new BibleBook {bookName = "Zechariah"},
                new BibleBook {bookName = "Malachi"},
                new BibleBook {bookName = "Matthew"},
                new BibleBook {bookName = "Mark"},
                new BibleBook {bookName = "Luke"},
                new BibleBook {bookName = "John"},
                new BibleBook {bookName = "Acts"},
                new BibleBook {bookName = "Romans"},
                new BibleBook {bookName = "1 Corinthians"},
                new BibleBook {bookName = "2 Corinthians"},
                new BibleBook {bookName = "Galatians"},
                new BibleBook {bookName = "Ephesians"},
                new BibleBook {bookName = "Philippians"},
                new BibleBook {bookName = "Colossians"},
                new BibleBook {bookName = "1 Thessalonians"},
                new BibleBook {bookName = "2 Thessalonians"},
                new BibleBook {bookName = "1 Timothy"},
                new BibleBook {bookName = "2 Timothy"},
                new BibleBook {bookName = "Titus"},
                new BibleBook {bookName = "Philemon"},
                new BibleBook {bookName = "Hebrews"},
                new BibleBook {bookName = "James"},
                new BibleBook {bookName = "1 Peter"},
                new BibleBook {bookName = "2 Peter"},
                new BibleBook {bookName = "1 John"},
                new BibleBook {bookName = "2 John"},
                new BibleBook {bookName = "3 John"},
                new BibleBook {bookName = "Jude"},
                new BibleBook {bookName = "Revelation"}
            };

            foreach (BibleBook b in bibleBooks)
            {
                context.BibleBook.Add(b);
            }
            context.SaveChanges();
        }
    }
}
