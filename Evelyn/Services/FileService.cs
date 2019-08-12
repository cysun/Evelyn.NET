using System.Threading.Tasks;
using Evelyn.Models;
using Microsoft.AspNetCore.Http;

namespace Evelyn.Services
{
    public class FileService
    {
        private readonly AppDbContext db;

        public FileService(AppDbContext db)
        {
            this.db = db;
        }

        public Models.File GetFile(int fileId)
        {
            return db.Files.Find(fileId);
        }

        public void AddFile(File file)
        {
            db.Files.Add(file);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
