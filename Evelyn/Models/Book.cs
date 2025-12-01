using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Evelyn.Models;

public class Book
{
    public int Id { get; init; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(255)]
    public string Author { get; set; }

    [MaxLength(8000)]
    public string Notes { get; set; }

    public List<Chapter> Chapters { get; set; } = new();

    public int MarkdownFileId { get; init; }
    [JsonIgnore] public File MarkdownFile { get; set; }

    public int? EBookFileId { get; set; }
    [JsonIgnore] public File EBookFile { get; set; }

    public int? CoverFileId { get; init; }
    [JsonIgnore] public File CoverFile { get; set; }

    public int? ThumbnailFileId { get; init; }
    [JsonIgnore] public File ThumbnailFile { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public DateTime? LastViewed { get; init; }

    public bool IsDeleted { get; init; }
}

[Table("Chapters")]
public class Chapter
{
    public int Id { get; init; }

    public int BookId { get; init; }
    public Book Book { get; init; }

    public int Number { get; init; }

    [Required]
    [MaxLength(255)]
    public string Name { get; init; }

    public int MarkdownFileId { get; init; }
    [JsonIgnore] public File MarkdownFile { get; set; }

    public int HtmlFileId { get; init; }
    [JsonIgnore] public File HtmlFile { get; set; }

    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
}
