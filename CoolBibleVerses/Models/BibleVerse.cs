namespace CoolBibleVerses.Models
{
    public class BibleVerse
    {
        public int Id { get; set; }
        public int BibleBookId { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string? Text { get; set; }
        public string Details { get; set; }
        public ICollection<VerseTag>? VerseTags { get; set; }
        public BibleBook BibleBook { get; set; }

        public BibleVerse()
        {
        }
    }
}
