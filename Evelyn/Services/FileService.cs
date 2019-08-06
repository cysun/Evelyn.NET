using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<Models.File> SaveFile(IFormFile uploadedFile)
        {
            var file = new Models.File
            {
                Name = System.IO.Path.GetFileName(uploadedFile.FileName),
                ContentType = uploadedFile.ContentType,
                Length = uploadedFile.Length

            };

            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                file.Content = memoryStream.ToArray();
            }

            db.Files.Add(file);
            db.SaveChanges();

            return file;
        }
    }
}
