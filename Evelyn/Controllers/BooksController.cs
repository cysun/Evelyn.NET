using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Evelyn.Models;
using Evelyn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class BooksController : Controller
    {
        private readonly FileService fileService;
        private readonly BookService bookService;
        private readonly EBookService ebookService;

        public BooksController(FileService fileService, BookService bookService, EBookService ebookService)
        {
            this.fileService = fileService;
            this.bookService = bookService;
            this.ebookService = ebookService;
        }

        public IActionResult List()
        {
            return View(bookService.GetBooks());
        }

        public IActionResult View(int id)
        {
            var book = bookService.GetBook(id);
            book.Chapters = book.Chapters.OrderBy(c => c.Number).ToList();
            if (book.Chapters.Count > 1)
                return View(book);
            else
                return LocalRedirect($"{Request.PathBase}/Book/{id}/Chapter/1");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Book());
        }

        [HttpPost]
        public IActionResult Add(Book book, IFormFile content, IFormFile cover)
        {
            var markdownFile = Models.File.FromUploadedFile(content);
            processContent(book, markdownFile);

            if (cover != null)
            {
                book.CoverFile = Models.File.FromUploadedFile(cover);
                book.ThumbnailFile = Models.File.ImageToThumbnail(book.CoverFile);
            }

            bookService.AddBook(book);
            bookService.SaveChanges();

            return RedirectToAction(nameof(View), new { id = book.BookId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(bookService.GetBook(id));
        }

        [HttpPost]
        public IActionResult Edit(int id, Book update, IFormFile content, IFormFile cover, bool isAppending)
        {
            var book = bookService.GetBook(id);
            book.Title = update.Title;
            book.Author = update.Author;
            book.Notes = update.Notes;

            if (content != null)
            {
                var markdownFile = Models.File.FromUploadedFile(content);
                processContent(book, markdownFile, isAppending);
                book.EBookFileId = null;
            }

            if (cover != null)
            {
                book.CoverFile = Models.File.FromUploadedFile(cover);
                book.ThumbnailFile = Models.File.ImageToThumbnail(book.CoverFile);
            }

            book.LastUpdated = DateTime.Now;
            bookService.SaveChanges();

            return RedirectToAction(nameof(View), new { id = book.BookId });
        }

        [HttpGet("/Book/{bookId}/Chapter/{chapterNumber}")]
        public IActionResult ViewChapter(int bookId, int chapterNumber)
        {
            var book = bookService.GetBook(bookId);
            var chapter = book.Chapters[chapterNumber - 1];
            ViewBag.Chapter = chapter;
            ViewBag.Html = fileService.GetFile(chapter.HtmlFileId);
            return View(book);
        }

        [HttpGet("/Book/{bookId}/EBook/Create")]
        public IActionResult CreateEBook(int bookId)
        {
            var book = bookService.GetBook(bookId);
            book.EBookFile = ebookService.CreateEPub(book);
            bookService.SaveChanges();
            return Ok(new { fileId = book.EBookFile.FileId });
        }

        private void processContent(Book book, Models.File markdownFile, bool IsAppending = false)
        {
            if (!IsAppending)
            {
                book.MarkdownFile = markdownFile;
                book.Chapters.Clear();
            }
            else
            {
                var oldFile = fileService.GetFile(book.MarkdownFileId);
                oldFile.Append(markdownFile);
                fileService.SaveChanges();
            }

            var chapters = new List<Chapter>();
            string chapterName = null;
            var stringBuilder = new StringBuilder();
            using (var reader = new StreamReader(markdownFile.OpenReadStream()))
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
