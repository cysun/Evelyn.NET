using System.IO.Compression;
using Evelyn.Models;
using Scriban;
using File = Evelyn.Models.File;

namespace Evelyn.Services;

public class EBookService
{
    // Set Zip entry to rw-r--rw-- on Unix systems
    private const int ZipExternalAttributes = 0b1_000_000_110_100_100 << 16;

    private readonly FileService _fileService;
    private readonly string _templatePath;

    public EBookService(IWebHostEnvironment env, FileService fileService)
    {
        _templatePath = Path.Combine(env.ContentRootPath, "Templates", "EPub");
        _fileService = fileService;
    }

    private void CopyToArchive(ZipArchive archive, string entryPath, string filePath,
        CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        var entry = archive.CreateEntry(entryPath, compressionLevel);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var inputStream = System.IO.File.OpenRead(filePath);
        using var outputStream = entry.Open();
        inputStream.CopyTo(outputStream);
    }

    private void WriteTextToArchive(ZipArchive archive, string entryPath, string content,
        CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        var entry = archive.CreateEntry(entryPath, compressionLevel);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var writer = new StreamWriter(entry.Open());
        writer.Write(content);
    }

    private void WriteBinaryToArchive(ZipArchive archive, string entryPath, byte[] content)
    {
        var entry = archive.CreateEntry(entryPath, CompressionLevel.NoCompression);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var writer = new BinaryWriter(entry.Open());
        writer.Write(content);
    }

    public File CreateEPub(Book book)
    {
        var buffer = new MemoryStream();
        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create))
        {
            CopyToArchive(archive, "mimeType", Path.Combine(_templatePath, "mimetype"), CompressionLevel.NoCompression);
            CopyToArchive(archive, "META-INF/container.xml", Path.Combine(_templatePath, "META-INF", "container.xml"));
            CopyToArchive(archive, "OPS/style.css", Path.Combine(_templatePath, "OPS", "style.css"));

            var chapterTemplate =
                Template.Parse(System.IO.File.ReadAllText(Path.Combine(_templatePath, "OPS", "chapter.xhtml")));
            foreach (var chapter in book.Chapters)
            {
                var content = chapterTemplate.Render(new
                    { book, chapter, file = _fileService.GetFile(chapter.HtmlFileId) });
                WriteTextToArchive(archive, $"OPS/chapter_{chapter.Number}.xhtml", content);
            }

            File cover = null;
            if (book.CoverFileId != null)
            {
                cover = _fileService.GetFile(book.CoverFileId);
                WriteBinaryToArchive(archive, $"OPS/{cover.Name}", cover.Content);
                var coverTemplate =
                    Template.Parse(System.IO.File.ReadAllText(Path.Combine(_templatePath, "OPS", "cover.xhtml")));
                WriteTextToArchive(archive, "OPS/cover.xhtml", coverTemplate.Render(new { book, cover }));
            }

            var packageTemplate =
                Template.Parse(System.IO.File.ReadAllText(Path.Combine(_templatePath, "OPS", "package.opf")));
            WriteTextToArchive(archive, "OPS/package.opf", packageTemplate.Render(new { book, cover }));

            var tocTemplate =
                Template.Parse(System.IO.File.ReadAllText(Path.Combine(_templatePath, "OPS", "toc.xhtml")));
            WriteTextToArchive(archive, "OPS/toc.xhtml", tocTemplate.Render(new { book }));
        }

        var epubFile = new File
        {
            Name = $"{book.Title}.epub",
            ContentType = "application/epub+zip",
            Content = buffer.ToArray()
        };
        epubFile.Length = epubFile.Content.Length;

        return epubFile;
    }
}
