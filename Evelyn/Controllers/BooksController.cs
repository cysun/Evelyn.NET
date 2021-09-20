using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Evelyn.Models;
using Evelyn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class BooksController : Controller
    {
        private readonly FileService _fileService;
        private readonly BookService _bookService;
        private readonly EBookService _ebookService;
        private readonly BookmarkService _bookmarkService;

        public BooksController(FileService fileService, BookService bookService,
            EBookService ebookService, BookmarkService bookmarkService)
        {
            _fileService = fileService;
            _bookService = bookService;
            _ebookService = ebookService;
            _bookmarkService = bookmarkService;
        }

        public IActionResult List(string term)
        {
            var books = term != null ? _bookService.SearchBooks(term) : _bookService.GetBooks();
            return View(books);
        }

        public IActionResult View(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var bookmark = _bookmarkService.GetAutoBookmark(userId, id);
            if (bookmark != null)
                return RedirectToAction("View", "Chapters", new
                {
                    id = bookmark.ChapterId,
                    paragraph = bookmark.Paragraph
                });

            var book = _bookService.GetBook(id);
            return RedirectToAction("View", "Chapters", new { id = book.Chapters[0].Id });
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

            _bookService.AddBook(book);
            _bookService.SaveChanges();

            return RedirectToAction(nameof(View), new { id = book.Id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_bookService.GetBook(id));
        }

        [HttpPost]
        public IActionResult Edit(int id, Book update, IFormFile content, IFormFile cover, bool isAppending)
        {
            var book = _bookService.GetBook(id);
            book.Title = update.Title;
            book.Author = update.Author;
            book.Notes = update.Notes;

            var fileIdsToDelete = new List<int>();

            if (content != null)
            {
                var markdownFile = Models.File.FromUploadedFile(content);
                processContent(book, markdownFile, isAppending, fileIdsToDelete);
            }

            if (cover != null)
            {
                if (book.CoverFileId != null)
                {
                    fileIdsToDelete.Add((int)book.CoverFileId);
                    fileIdsToDelete.Add((int)book.ThumbnailFileId);
                }
                book.CoverFile = Models.File.FromUploadedFile(cover);
                book.ThumbnailFile = Models.File.ImageToThumbnail(book.CoverFile);
            }

            if (book.EBookFileId != null)
            {
                fileIdsToDelete.Add((int)book.EBookFileId);
                book.EBookFileId = null;
            }

            book.LastUpdated = DateTime.Now;
            _bookService.SaveChanges();

            if (fileIdsToDelete.Count > 0)
                _fileService.DeleteFiles(fileIdsToDelete);

            return RedirectToAction(nameof(View), new { id = book.Id });
        }

        public IActionResult Delete(int id)
        {
            var book = _bookService.GetBook(id);

            var fileIdsToDelete = new List<int>();
            fileIdsToDelete.Add(book.MarkdownFileId);
            if (book.EBookFileId != null) fileIdsToDelete.Add((int)book.EBookFileId);
            if (book.CoverFileId != null) fileIdsToDelete.Add((int)book.CoverFileId);
            if (book.ThumbnailFileId != null) fileIdsToDelete.Add((int)book.ThumbnailFileId);
            foreach (var chapter in book.Chapters)
            {
                fileIdsToDelete.Add(chapter.MarkdownFileId);
                fileIdsToDelete.Add(chapter.HtmlFileId);
            }

            _bookService.DeleteBook(book);
            _fileService.DeleteFiles(fileIdsToDelete);

            return RedirectToAction(nameof(List));
        }

        public IActionResult Download(int id)
        {
            var book = _bookService.GetBook(id);
            var file = _fileService.GetFile(book.MarkdownFileId);
            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }

        // Due a previous bug, the text or markdown file of a book may be incomplete
        // (i.e. not including chapters that are supposed to be appended later), so
        // instead of Download, this Markdown action should be used to download a
        // markdown verson of the book.
        public IActionResult Markdown(int id)
        {
            var book = _bookService.GetBook(id);
            var title = Encoding.UTF8.GetBytes($"# {book.Title}\n");
            var author = Encoding.UTF8.GetBytes($"### {book.Author}\n");
            var chapters = book.Chapters.Select(c => _fileService.GetFile(c.MarkdownFileId).Content);

            byte[] content = new byte[title.Length + author.Length + chapters.Select(c => c.Length).Sum()];

            Buffer.BlockCopy(title, 0, content, 0, title.Length);
            Buffer.BlockCopy(author, 0, content, title.Length, author.Length);

            var offset = title.Length + author.Length;
            foreach (var chapter in chapters)
            {
                Buffer.BlockCopy(chapter, 0, content, offset, chapter.Length);
                offset += chapter.Length;
            }

            return File(content, "text/markdown", $"{book.Id}.txt");
        }

        public IActionResult EBook(int id)
        {
            var book = _bookService.GetBook(id);
            var file = book.EBookFileId != null ? _fileService.GetFile(book.EBookFileId)
                : _ebookService.CreateEPub(book);

            if (book.EBookFileId == null)
            {
                book.EBookFile = file;
                _bookService.SaveChanges();
            }

            return File(file.OpenReadStream(), file.ContentType, file.Name);
        }

        private void processContent(Book book, Models.File markdownFile, bool IsAppending = false,
            List<int> fileIdsToDelete = null)
        {
            if (!IsAppending)
            {
                if (fileIdsToDelete != null)
                {
                    fileIdsToDelete.Add(book.MarkdownFileId);
                    foreach (var chapter in book.Chapters)
                    {
                        fileIdsToDelete.Add(chapter.MarkdownFileId);
                        fileIdsToDelete.Add(chapter.HtmlFileId);
                    }
                }
                book.MarkdownFile = markdownFile;
                book.Chapters.Clear();
            }
            else
            {
                var oldFile = _fileService.GetFile(book.MarkdownFileId);
                oldFile.Append(markdownFile);
                _fileService.SaveChanges();
            }

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
