using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Evelyn.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Evelyn.Services
{
    public class UserService
    {
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
        }

        public ClaimsIdentity Authenticate(string username, string password)
        {
            var user = GetUser(username);
            if (user == null || !user.VerifyPassword(password))
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public User GetUser(string name)
        {
            return db.Users.Where(u => u.Name.ToUpper() == name.ToUpper()).SingleOrDefault();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
