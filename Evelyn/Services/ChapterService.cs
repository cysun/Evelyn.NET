using System.Linq;
using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class ChapterService
    {
        private readonly AppDbContext _db;

        public ChapterService(AppDbContext db)
        {
            _db = db;
        }

        public Chapter GetChapter(int id)
        {
            var chapter = _db.Chapters.Where(c => c.Id == id)
                .Include(c => c.Book).ThenInclude(b => b.Chapters)
                .SingleOrDefault();

            if (chapter != null && chapter.Book.Chapters.Count > 1)
                chapter.Book.Chapters = chapter.Book.Chapters.OrderBy(c => c.Number).ToList();

            return chapter;
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
