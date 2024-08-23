namespace CoolBibleVerses.Models
{
    public class VerseTag
    {
        public int TagId { get; set; }
        public int BibleVerseId { get; set; }
        public BibleVerse BibleVerse { get; set; }
        public Tag Tag { get; set; }

        public VerseTag()
        {

        }
    }
    
}
