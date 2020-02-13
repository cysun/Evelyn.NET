using System;
using System.Security.Claims;
using Evelyn.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class ChaptersController : Controller
    {
        private readonly FileService _fileService;
        private readonly ChapterService _chapterService;
        private readonly BookmarkService _bookmarkService;

        public ChaptersController(FileService fileService, ChapterService chapterService,
            BookmarkService bookmarkService)
        {
            _fileService = fileService;
            _chapterService = chapterService;
            _bookmarkService = bookmarkService;
        }

        public IActionResult View(int id)
        {
            var chapter = _chapterService.GetChapter(id);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var bookmark = _bookmarkService.GetAutoBookmark(userId, chapter.Book.Id);
            if (bookmark == null)
            {
                bookmark = new Models.Bookmark
                {
                    UserId = userId,
                    ChapterId = chapter.Id
                };
                _bookmarkService.AddBookmark(bookmark);
            }
            else
            {
                bookmark.ChapterId = chapter.Id;
                bookmark.Timestamp = DateTime.Now;
            }
            _bookmarkService.SaveChanges();

            ViewBag.Html = _fileService.GetFile(chapter.HtmlFileId);
            return View(chapter);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var chapter = _chapterService.GetChapter(id);
            ViewBag.Markdown = _fileService.GetFile(chapter.MarkdownFileId);
            return View(chapter);
        }

        [HttpPost]
        public IActionResult EditChapter(int id, string text)
        {
            var chapter = _chapterService.GetChapter(id);
            var markdownFile = _fileService.GetFile(chapter.MarkdownFileId);
            markdownFile.Text = text;
            var htmlFile = _fileService.GetFile(chapter.HtmlFileId);
            htmlFile.Text = Markdown.ToHtml(text);
            _fileService.SaveChanges();
            return RedirectToAction(nameof(View), new { Id = id });
        }
    }
}
