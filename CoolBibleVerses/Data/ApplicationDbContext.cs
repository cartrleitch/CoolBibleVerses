﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoolBibleVerses.Models;

namespace CoolBibleVerses.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CoolBibleVerses.Models.BibleVerse> BibleVerse { get; set; } = default!;
        public DbSet<CoolBibleVerses.Models.VerseTag> VerseTag { get; set; }
        public DbSet<CoolBibleVerses.Models.Tag> Tag { get; set; }
        public DbSet<CoolBibleVerses.Models.BibleBook> BibleBook { get; set; }
    }
}
