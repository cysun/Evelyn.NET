using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class BookmarkService
    {
        private readonly AppDbContext _db;

        public BookmarkService(AppDbContext db)
        {
            _db = db;
        }

        public List<Bookmark> GetBookmarks(int userId)
        {
            return _db.Bookmarks.Where(b => b.UserId == userId)
                .Include(b => b.Chapter).ThenInclude(c => c.Book)
                .OrderBy(b => b.IsManual).ThenByDescending(b => b.Timestamp).ToList();
        }

        public Bookmark GetBookmark(int userId, int chapterId)
        {
            return _db.Bookmarks.Where(b => b.UserId == userId && b.ChapterId == chapterId)
                .OrderBy(b => b.IsManual).FirstOrDefault();
        }

        public Bookmark SetBookmark(int userId, int chapterId, int paragraph = 1)
        {
            var bookmark = _db.Bookmarks
                .Where(b => b.UserId == userId && b.ChapterId == chapterId && b.IsManual)
                .SingleOrDefault();

            if (bookmark == null)
            {
                bookmark = new Bookmark
                {
                    UserId = userId,
                    ChapterId = chapterId,
                    Paragraph = paragraph,
                    IsManual = true
                };
                _db.Bookmarks.Add(bookmark);
            }
            else
            {
                bookmark.Paragraph = paragraph;
            }
            _db.SaveChanges();
            return bookmark;
        }

        public void DeleteBookmark(int id)
        {
            var bookmark = new Bookmark { Id = id };
            _db.Bookmarks.Attach(bookmark);
            _db.Bookmarks.Remove(bookmark);
            _db.SaveChanges();
        }

        public Bookmark GetAutoBookmark(int userId, int bookId)
        {
            return _db.Bookmarks.Include(b => b.Chapter)
                .Where(b => b.UserId == userId && b.Chapter.BookId == bookId && b.IsManual == false)
                .SingleOrDefault();
        }

        public void SetAutoBookmark(int userId, int bookId, int chapterId, int paragraph = 1)
        {
            var bookmark = _db.Bookmarks.Include(b => b.Chapter)
                .Where(b => b.UserId == userId && b.Chapter.BookId == bookId && b.IsManual == false)
                .SingleOrDefault();

            if (bookmark == null)
            {
                bookmark = new Bookmark
                {
                    UserId = userId,
                    ChapterId = chapterId,
                    Paragraph = paragraph
                };
                _db.Bookmarks.Add(bookmark);
            }
            else
            {
                bookmark.ChapterId = chapterId;
                bookmark.Paragraph = paragraph;
                bookmark.Timestamp = DateTime.UtcNow;
            }
            _db.SaveChanges();
        }
    }
}
