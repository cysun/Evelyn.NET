using System.Security.Claims;
using System.Threading.Tasks;
using Evelyn.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserService userService;
        public AccountController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return User.Identity.IsAuthenticated ? RedirectToAction("List", "Bookmarks") : (IActionResult)View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            var identity = userService.Authenticate(username, password);
            if (identity == null)
                return RedirectToAction(nameof(Login));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true
                });

            return string.IsNullOrWhiteSpace(returnUrl) ?
                RedirectToAction("List", "Bookmarks") :
                (IActionResult)LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
