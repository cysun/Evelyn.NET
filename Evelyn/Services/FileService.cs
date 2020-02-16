using System;
using Evelyn.Models;

namespace Evelyn.Services
{
    public class FileService
    {
        private readonly AppDbContext _db;

        public FileService(AppDbContext db)
        {
            _db = db;
        }

        public File GetFile(int? id)
        {
            if (id == null) throw new ArgumentNullException();
            return _db.Files.Find(id);
        }

        public void DeleteFile(int id)
        {
            var file = new File { Id = id };
            _db.Files.Attach(file);
            _db.Files.Remove(file);
            _db.SaveChanges();
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
