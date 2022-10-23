using System.IO.Compression;
using Scriban;

namespace Evelyn.Services;

public class EBookService
{
    private readonly string templatePath;

    private readonly FileService fileService;

    // Set Zip entry to rw-r--rw-- on Unix systems
    public const int ZipExternalAttributes = 0b1_000_000_110_100_100 << 16;

    public EBookService(IWebHostEnvironment env, FileService fileService)
    {
        this.templatePath = Path.Combine(env.ContentRootPath, "Templates", "EPub");
        this.fileService = fileService;
    }

    private void copyToArchive(ZipArchive archive, string entryPath, string filePath,
        CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        var entry = archive.CreateEntry(entryPath, compressionLevel);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var inputStream = File.OpenRead(filePath);
        using var outputStream = entry.Open();
        inputStream.CopyTo(outputStream);
    }

    private void writeTextToArchive(ZipArchive archive, string entryPath, string content,
        CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        var entry = archive.CreateEntry(entryPath, compressionLevel);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var writer = new StreamWriter(entry.Open());
        writer.Write(content);
    }

    private void writeBinaryToArchive(ZipArchive archive, string entryPath, byte[] content)
    {
        var entry = archive.CreateEntry(entryPath, CompressionLevel.NoCompression);
        entry.ExternalAttributes = ZipExternalAttributes;
        using var writer = new BinaryWriter(entry.Open());
        writer.Write(content);
    }

    public Models.File CreateEPub(Models.Book book)
    {
        var buffer = new MemoryStream();
        using (ZipArchive archive = new ZipArchive(buffer, ZipArchiveMode.Create))
        {
            copyToArchive(archive, "mimeType", Path.Combine(templatePath, "mimetype"), CompressionLevel.NoCompression);
            copyToArchive(archive, "META-INF/container.xml", Path.Combine(templatePath, "META-INF", "container.xml"));
            copyToArchive(archive, "OPS/style.css", Path.Combine(templatePath, "OPS", "style.css"));

            var chapterTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "chapter.xhtml")));
            foreach (var chapter in book.Chapters)
            {
                string content = chapterTemplate.Render(new { book, chapter, file = fileService.GetFile(chapter.HtmlFileId) });
                writeTextToArchive(archive, $"OPS/chapter_{chapter.Number}.xhtml", content);
            }

            Models.File cover = null;
            if (book.CoverFileId != null)
            {
                cover = fileService.GetFile(book.CoverFileId);
                writeBinaryToArchive(archive, $"OPS/{cover.Name}", cover.Content);
                var coverTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "cover.xhtml")));
                writeTextToArchive(archive, "OPS/cover.xhtml", coverTemplate.Render(new { book, cover }));
            }

            var packageTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "package.opf")));
            writeTextToArchive(archive, "OPS/package.opf", packageTemplate.Render(new { book, cover }));

            var tocTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "toc.xhtml")));
            writeTextToArchive(archive, "OPS/toc.xhtml", tocTemplate.Render(new { book }));
        }

        var epubFile = new Models.File
        {
            Name = $"{book.Title}.epub",
            ContentType = "application/epub+zip",
            Content = buffer.ToArray()
        };
        epubFile.Length = epubFile.Content.Length;

        return epubFile;
    }
}
