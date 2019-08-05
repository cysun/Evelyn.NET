using System.ComponentModel.DataAnnotations;

namespace Evelyn.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Hash { get; set; }
    }
}
