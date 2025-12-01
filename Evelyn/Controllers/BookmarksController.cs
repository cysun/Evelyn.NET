using System.Security.Claims;
using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers;

public class BookmarksController : Controller
{
    private readonly BookmarkService _bookmarkService;

    public BookmarksController(BookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    public IActionResult List()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        return View(_bookmarkService.GetBookmarks(int.Parse(userId)));
    }

    public IActionResult Set(int chapterId, int paragraph = 1)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        _bookmarkService.SetBookmark(userId, chapterId, paragraph);
        return Ok();
    }

    public IActionResult Delete(int id)
    {
        _bookmarkService.DeleteBookmark(id);
        return RedirectToAction(nameof(List));
    }

    public IActionResult AutoBookmark(int bookId, int chapterId, int paragraph = 1)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        _bookmarkService.SetAutoBookmark(userId, bookId, chapterId, paragraph);
        return Ok();
    }
}
