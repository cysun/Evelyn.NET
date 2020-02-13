using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Evelyn.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Evelyn.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public ClaimsIdentity Authenticate(string username, string password)
        {
            var user = GetUser(username);
            if (user == null || !user.VerifyPassword(password))
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public User GetUser(string name)
        {
            return _db.Users.Where(u => u.Name.ToUpper() == name.ToUpper()).SingleOrDefault();
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
