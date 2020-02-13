using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Evelyn.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }
        public string Notes { get; set; }

        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public int ChaptersCount { get; set; }

        public int MarkdownFileId { get; set; }
        [JsonIgnore]
        public File MarkdownFile { get; set; }

        public int? EBookFileId { get; set; }
        [JsonIgnore]
        public File EBookFile { get; set; }

        public int? CoverFileId { get; set; }
        [JsonIgnore]
        public File CoverFile { get; set; }

        public int? ThumbnailFileId { get; set; }
        [JsonIgnore]
        public File ThumbnailFile { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }

    [Table("Chapters")]
    public class Chapter
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public int Number { get; set; }
        public string Name { get; set; }

        public int MarkdownFileId { get; set; }
        [JsonIgnore]
        public File MarkdownFile { get; set; }

        public int HtmlFileId { get; set; }
        [JsonIgnore]
        public File HtmlFile { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
