namespace Evelyn.Models;

public class Bookmark
{
    public int Id { get; init; }

    public int UserId { get; init; }
    public User User { get; init; }

    public int ChapterId { get; set; }
    public Chapter Chapter { get; init; }

    public int Paragraph { get; set; } = 1;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsManual { get; init; }
}
