using System.Security.Claims;
using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserService userService;

        public ProfileController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(string password)
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var user = userService.GetUser(username);
            user.Password = password;
            userService.SaveChanges();
            return Redirect("Edit?Saved");
        }
    }
}
