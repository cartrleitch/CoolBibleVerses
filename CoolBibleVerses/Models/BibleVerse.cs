namespace CoolBibleVerses.Models
{
    public class BibleVerse
    {
        public int Id { get; set; }
        public string Book { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string Text { get; set; }

        //public List<string> Tags { get; set; } = new List<string>();

        // maybe use ESV API to get the text to populate the Text

        public BibleVerse()
        {
        }
    }
}
