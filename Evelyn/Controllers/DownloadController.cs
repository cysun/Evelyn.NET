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

        public IActionResult MarkdownFiles()
        {
            var buffer = new MemoryStream();
            ZipArchive archive = new ZipArchive(buffer, ZipArchiveMode.Create);

            var books = _bookService.GetBooks();
            foreach (var book in books)
            {
                var file = _fileService.GetFile(book.MarkdownFileId);
                var entry = archive.CreateEntry(file.Name);
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    writer.Write(file.Text);
                }
            }

            return File(buffer.ToArray(), "application/zip", "AllMarkdownFiles.zip");
        }

        public IActionResult BooksMetadata()
        {
            var books = _bookService.GetBooks().OrderBy(b => b.BookId);
            // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return File(JsonSerializer.SerializeToUtf8Bytes(books, options),
                "application/json", "AllBooksMetadata.json");
        }
    }
}
