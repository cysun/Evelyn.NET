using System.Collections.Generic;
using System.Linq;
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
                .OrderByDescending(b => b.Timestamp).ToList();
        }

        public Bookmark GetAutoBookmark(int userId, int bookId)
        {
            return _db.Bookmarks.Include(b => b.Chapter)
                .Where(b => b.UserId == userId && b.Chapter.BookId == bookId && b.IsManual == false)
                .SingleOrDefault();
        }

        public void AddBookmark(Bookmark bookmark) => _db.Bookmarks.Add(bookmark);

        public void DeleteBookmark(int id) => _db.Bookmarks.Remove(new Bookmark { Id = id });

        public void SaveChanges() => _db.SaveChanges();
    }
}
