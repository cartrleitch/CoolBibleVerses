using System.ComponentModel.DataAnnotations;

namespace CoolBibleVerses.Models
{
    public class BibleVerse
    {
        public int Id { get; set; }
        public int BibleBookId { get; set; }
        public int Chapter { get; set; }
        [Display(Name = "Verse(s)")]
        public int? Verse { get; set; }
        public int? VerseEnd { get; set; }
        public string? Text { get; set; }
        public string Details { get; set; }
        public ICollection<VerseTag>? VerseTags { get; set; }
        public BibleBook? BibleBook { get; set; }
        [Display(Name = "Created By")]
        public string? EnteredBy { get; set; }
        [Display(Name = "Date Created")]
        public DateTime? DateEntered { get; set; }


        public BibleVerse()
        {
        }
    }
}
