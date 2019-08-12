using System.Collections.Generic;
using System.IO;
using System.Text;
using Evelyn.Models;
using Evelyn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService bookService;
        private readonly FileService fileService;

        public BooksController(BookService bookService, FileService fileService)
        {
            this.bookService = bookService;
            this.fileService = fileService;
        }

        public IActionResult List()
        {
            return View(bookService.GetBooks());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Book());
        }

        [HttpPost]
        public IActionResult Add(Book book, IFormFile content, IFormFile cover)
        {
            if (content != null)
            {
                var markdownFile = Models.File.FromUploadedFile(content);
                book.MarkdownFile = markdownFile;
                splitChapters(book, markdownFile);
            }

            if (cover != null)
            {
                book.CoverFile = Models.File.FromUploadedFile(cover);
                book.ThumbnailFile = Models.File.ImageToThumbnail(book.CoverFile);
            }

            bookService.AddBook(book);
            bookService.SaveChanges();

            return RedirectToAction(nameof(List));
        }

        private void splitChapters(Book book, Models.File markdown, bool IsAppending = false)
        {
            if (!IsAppending) book.Chapters.Clear();
            var chapters = new List<Chapter>();

            string chapterName = null;
            var stringBuilder = new StringBuilder();
            using (var reader = new StreamReader(markdown.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("# ") || line.StartsWith("### ")) continue;
                    if (stringBuilder.Length == 0 && string.IsNullOrWhiteSpace(line)) continue;

                    if (line.StartsWith("## "))
                    {
                        if (stringBuilder.Length > 0) createChapter();
                        chapterName = line.Substring(3);
                    }
                    stringBuilder.AppendLine(line);
                }
                createChapter();
            }

            void createChapter()
            {
                var chapterNumber = book.Chapters.Count + 1;
                var chapter = new Chapter
                {
                    Book = book,
                    Number = chapterNumber,
                    Name = chapterName ?? book.Title
                };

                chapter.MarkdownFile = new Models.File
                {
                    Name = chapter.Name,
                    ContentType = "text/markdown",
                    Text = stringBuilder.ToString()
                };
                chapter.MarkdownFile.Length = chapter.MarkdownFile.Content.Length;
                chapter.HtmlFile = Models.File.MarkdownToHtml(chapter.MarkdownFile);

                book.Chapters.Add(chapter);
                stringBuilder.Clear();
            }
        }
    }
}
