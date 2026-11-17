using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoolBibleVerses.Models;
using Microsoft.AspNetCore.Identity;

namespace CoolBibleVerses.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BibleVerse> BibleVerse { get; set; } = default!;
        public DbSet<VerseTag> VerseTag { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<BibleBook> BibleBook { get; set; }
        
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
