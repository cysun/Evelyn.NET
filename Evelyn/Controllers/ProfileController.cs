using System.Security.Claims;
using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers;

public class ProfileController : Controller
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Edit() => View();

    [HttpPost]
    public IActionResult Edit(string password)
    {
        var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        var user = _userService.GetUser(username);
        user.Password = password;
        _userService.SaveChanges();
        return Redirect("Edit?Saved");
    }
}
