using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public IActionResult Delete(int bookmarkId)
        {
            bookmarkService.DeleteBookmark(bookmarkId);
            return RedirectToAction(nameof(List));
        }
    }
}
