namespace CoolBibleVerses.Models
{
    public class VerseTag
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int BibleVerseId { get; set; }
        public BibleVerse BibleVerse { get; set; }

        public VerseTag()
        {

        }
    }
    
}
