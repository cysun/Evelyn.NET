﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evelyn.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }
        public List<Chapter> Chapters { get; set; }

        public int? MarkdownFileId { get; set; }
        public TextFile MarkdownFile { get; set; }

        public int? EBookFileId { get; set; }
        public File EBookFile { get; set; }

        public int? CoverFileId { get; set; }
        public File CoverFile { get; set; }

        public int? ThumbnailFileId { get; set; }
        public File ThumbnailFile { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    [Table("Chapters")]
    public class Chapter
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public int? MarkdownFileId { get; set; }
        public TextFile MarkdownFile { get; set; }

        public int? HtmlFileId { get; set; }
        public TextFile HtmlFile { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
