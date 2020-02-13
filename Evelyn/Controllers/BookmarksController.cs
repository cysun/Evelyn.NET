using System.Linq;
using System.Security.Claims;
using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly BookmarkService bookmarkService;

        public BookmarksController(BookmarkService bookmarkService)
        {
            this.bookmarkService = bookmarkService;
        }

        public IActionResult List()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return View(bookmarkService.GetBookmarks(int.Parse(userId)));
        }

        public IActionResult Add(int chapterId, int paragraph = 1)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            bookmarkService.AddBookmark(userId, chapterId, paragraph);
            return Ok();
        }

        public IActionResult Delete(int id)
        {
            bookmarkService.DeleteBookmark(id);
            return RedirectToAction(nameof(List));
        }

        public IActionResult AutoBookmark(int bookId, int chapterId, int paragraph = 1)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            bookmarkService.AutoBookmark(userId, bookId, chapterId, paragraph);
            return Ok();
        }
    }
}
