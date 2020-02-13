using System;
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

        public Bookmark AddBookmark(int userId, int chapterId, int paragraph = 1)
        {
            var bookmark = new Bookmark
            {
                UserId = userId,
                ChapterId = chapterId,
                Paragraph = paragraph,
                IsManual = true
            };
            _db.Bookmarks.Add(bookmark);
            _db.SaveChanges();
            return bookmark;
        }

        public void DeleteBookmark(int id)
        {
            var bookmark = _db.Bookmarks.Find(id);
            _db.Bookmarks.Remove(bookmark);
            _db.SaveChanges();
        }

        public void AutoBookmark(int userId, int bookId, int chapterId, int paragraph = 1)
        {
            var bookmark = _db.Bookmarks.Include(b => b.Chapter)
                .Where(b => b.UserId == userId && b.Chapter.BookId == bookId && b.IsManual == false)
                .SingleOrDefault();

            if (bookmark == null)
            {
                bookmark = new Models.Bookmark
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
                bookmark.Timestamp = DateTime.Now;
            }
            _db.SaveChanges();
        }
    }
}
