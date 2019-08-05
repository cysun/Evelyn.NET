using System;

namespace Evelyn.Models
{
    public class Bookmark
    {
        public int BookmarkId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public int ChapterNumber { get; set; }

        public int Position { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
