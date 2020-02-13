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

        public File GetFile(int? fileId)
        {
            if (fileId == null) throw new ArgumentNullException();
            return _db.Files.Find(fileId);
        }

        public void AddFile(File file) => _db.Files.Add(file);

        public void SaveChanges() => _db.SaveChanges();
    }
}
