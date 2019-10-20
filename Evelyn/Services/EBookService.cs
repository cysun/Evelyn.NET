using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;
using Scriban;

namespace Evelyn.Services
{
    public class EBookService
    {
        private readonly string templatePath;

        private readonly FileService fileService;

        public EBookService(IWebHostEnvironment env, FileService fileService)
        {
            this.templatePath = Path.Combine(env.ContentRootPath, "Templates", "EPub");
            this.fileService = fileService;
        }

        public Models.File CreateEPub(Models.Book book)
        {
            var tempDir = createTempDirectory();
            var metaInfDir = tempDir.CreateSubdirectory("META-INF");
            var opsDir = tempDir.CreateSubdirectory("OPS");
            string content;

            File.Copy(Path.Combine(templatePath, "mimetype"), Path.Combine(tempDir.FullName, "mimetype"));
            File.Copy(Path.Combine(templatePath, "META-INF", "container.xml"), Path.Combine(metaInfDir.FullName, "container.xml"));
            File.Copy(Path.Combine(templatePath, "OPS", "style.css"), Path.Combine(opsDir.FullName, "style.css"));

            var chapterTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "chapter.xhtml")));
            foreach (var chapter in book.Chapters)
            {
                content = chapterTemplate.Render(new { book, chapter, file = fileService.GetFile(chapter.HtmlFileId) });
                File.WriteAllText(Path.Combine(opsDir.FullName, $"chapter_{chapter.Number}.xhtml"), content);
            }

            Models.File cover = null;
            if (book.CoverFileId != null)
            {
                cover = fileService.GetFile(book.CoverFileId);
                File.WriteAllBytes(Path.Combine(opsDir.FullName, cover.Name), cover.Content);
                var coverTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "cover.xhtml")));
                content = coverTemplate.Render(new { book, cover });
                File.WriteAllText(Path.Combine(opsDir.FullName, "cover.xhtml"), content);
            }

            var packageTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "package.opf")));
            content = packageTemplate.Render(new { book, cover });
            File.WriteAllText(Path.Combine(opsDir.FullName, "package.opf"), content);

            var tocTemplate = Template.Parse(File.ReadAllText(Path.Combine(templatePath, "OPS", "toc.xhtml")));
            content = tocTemplate.Render(new { book });
            File.WriteAllText(Path.Combine(opsDir.FullName, "toc.xhtml"), content);

            var epubPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            ZipFile.CreateFromDirectory(tempDir.FullName, epubPath);

            var epubFile = new Models.File
            {
                Name = $"{book.Title}.epub",
                ContentType = "application/epub+zip",
                Content = File.ReadAllBytes(epubPath)
            };
            epubFile.Length = epubFile.Content.Length;

            return epubFile;
        }

        private DirectoryInfo createTempDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            return Directory.CreateDirectory(tempDirectory);
        }
    }
}
