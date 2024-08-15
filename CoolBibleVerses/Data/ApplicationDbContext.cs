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
    }
}
