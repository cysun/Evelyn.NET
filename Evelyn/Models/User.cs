using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evelyn.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Hash { get; set; }

    [NotMapped]
    public string Password
    {
        set => Hash = BCrypt.Net.BCrypt.HashPassword(value);
    }

    public bool VerifyPassword(string password) => BCrypt.Net.BCrypt.Verify(password, Hash);
}
