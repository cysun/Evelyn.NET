using System.Collections.Generic;
using System.Linq;
using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class BookmarkService
    {
        private readonly AppDbContext db;

        public BookmarkService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Bookmark> GetBookmarks(int userId)
        {
            return db.Bookmarks.Where(b => b.UserId == userId).Include(b => b.Book)
                .OrderByDescending(b => b.Timestamp).ToList();
        }

        public Bookmark GetBookmark(int userId, int bookId)
        {
            return db.Bookmarks.Where(b => b.UserId == userId && b.BookId == bookId)
                .SingleOrDefault();
        }

        public void DeleteBookmark(int bookmarkId)
        {
            db.Bookmarks.Remove(new Bookmark { BookmarkId = bookmarkId });
            db.SaveChanges();
        }

        public void SaveChanges() => db.SaveChanges();
    }
}
