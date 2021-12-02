namespace Evelyn.Models
{
    public class Bookmark
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public int Paragraph { get; set; } = 1;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsManual { get; set; } = false;
    }
}
