using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evelyn.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Hash { get; set; }

        [NotMapped]
        public string Password
        {
            set => Hash = BCrypt.Net.BCrypt.HashPassword(value);
        }

        public bool VerifyPassword(string password) => BCrypt.Net.BCrypt.Verify(password, Hash);
    }
}
