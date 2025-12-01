using System.Security.Claims;
using Evelyn.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login() =>
        User.Identity is { IsAuthenticated: true } ? RedirectToAction("List", "Books") : View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string returnUrl)
    {
        var identity = _userService.Authenticate(username, password);
        if (identity == null)
            return RedirectToAction(nameof(Login));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true
            });

        return string.IsNullOrWhiteSpace(returnUrl) ? RedirectToAction("List", "Books") : LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
