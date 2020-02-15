using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class DownloadController : Controller
    {
        private readonly BookService _bookService;
        private readonly FileService _fileService;

        public DownloadController(BookService bookService, FileService fileService)
        {
            _bookService = bookService;
            _fileService = fileService;
        }

        private void addToArchive(ZipArchive archive, int fileId, bool isText = true)
        {
            var file = _fileService.GetFile(fileId);
            var entry = archive.CreateEntry(file.Name);
            if (isText)
            {
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    writer.Write(file.Text);
                }
            }
            else
            {
                using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                {
                    writer.Write(file.Content);
                }
            }
        }

        public IActionResult AllFiles()
        {
            var buffer = new MemoryStream();
            ZipArchive archive = new ZipArchive(buffer, ZipArchiveMode.Create);

            var books = _bookService.GetBooks();
            foreach (var book in books)
            {
                addToArchive(archive, book.MarkdownFileId);
                if (book.CoverFileId != null)
                    addToArchive(archive, (int)book.CoverFileId, false);
            }

            return File(buffer.ToArray(), "application/zip", "AllFiles.zip");
        }

        public IActionResult AllMetadata()
        {
            var books = _bookService.GetBooks().OrderBy(b => b.Id);
            // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return File(JsonSerializer.SerializeToUtf8Bytes(books, options),
                "application/json", "AllMetadata.json");
        }
    }
}
