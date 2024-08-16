namespace CoolBibleVerses.Models
{
    public class VerseTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int BibleVerseId { get; set; }
        public BibleVerse BibleVerse { get; set; }

        public VerseTag()
        {

        }
    }
    
}
