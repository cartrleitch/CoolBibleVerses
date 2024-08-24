using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BibleVerse>().ToTable("BibleVerse");
            modelBuilder.Entity<VerseTag>().ToTable("VerseTag");
            modelBuilder.Entity<Tag>().ToTable("Tag");
            modelBuilder.Entity<BibleBook>().ToTable("BibleBook");

            modelBuilder.Entity<VerseTag>()
                .HasKey(vt => new { vt.TagId, vt.BibleVerseId });

            base.OnModelCreating(modelBuilder);
        }
    }

    
}
