using System.Security.Claims;
using Evelyn.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class ChaptersController : Controller
    {
        private readonly FileService _fileService;
        private readonly BookService _bookService;
        private readonly ChapterService _chapterService;
        private readonly BookmarkService _bookmarkService;

        public ChaptersController(FileService fileService, BookService bookService,
            ChapterService chapterService, BookmarkService bookmarkService)
        {
            _fileService = fileService;
            _bookService = bookService;
            _chapterService = chapterService;
            _bookmarkService = bookmarkService;
        }

        public IActionResult List(int bookId)
        {
            return View(_bookService.GetBook(bookId));
        }

        public IActionResult View(int id, int paragraph = 1)
        {
            var chapter = _chapterService.GetChapter(id);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var bookmark = _bookmarkService.GetBookmark(userId, id);

            ViewBag.Paragraph = paragraph == 1 && bookmark != null ? bookmark.Paragraph : paragraph;
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
