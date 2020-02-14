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
            return _db.Chapters.Where(c => c.Id == id)
                .Include(c => c.Book).ThenInclude(b => b.Chapters)
                .SingleOrDefault();
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
