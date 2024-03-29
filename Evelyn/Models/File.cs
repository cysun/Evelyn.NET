using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Markdig;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Evelyn.Models
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public byte[] Content { get; set; }

        [NotMapped]
        public string Text
        {
            get => Encoding.UTF8.GetString(Content);
            set => Content = Encoding.UTF8.GetBytes(value);
        }

        public Stream OpenReadStream()
        {
            return new MemoryStream(Content, false);
        }

        public void Append(File another)
        {
            byte[] newContent = new byte[Content.Length + another.Content.Length];
            Buffer.BlockCopy(Content, 0, newContent, 0, Content.Length);
            Buffer.BlockCopy(another.Content, 0, newContent, Content.Length, another.Content.Length);
            Length += another.Length;
            Timestamp = DateTime.UtcNow;
            Content = newContent;
        }

        public static File FromUploadedFile(IFormFile uploadedFile)
        {
            var file = new File
            {
                Name = Path.GetFileName(uploadedFile.FileName),
                ContentType = uploadedFile.ContentType,
                Length = uploadedFile.Length
            };

            using (var memoryStream = new MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);
                file.Content = memoryStream.ToArray();
            }
            file.Length = file.Content.Length;

            return file;
        }

        public static File MarkdownToHtml(File markdownFile)
        {
            var htmlFile = new File
            {
                Name = markdownFile.Name,
                ContentType = "text/html",
                Text = Markdown.ToHtml(markdownFile.Text)
            };
            htmlFile.Length = htmlFile.Content.Length;

            return htmlFile;
        }

        public static File ImageToThumbnail(File imageFile)
        {
            var imageName = Path.GetFileNameWithoutExtension(imageFile.Name);
            var imageExtension = Path.GetExtension(imageFile.Name);
            var thumbnail = new File
            {
                Name = $"{imageName} - thumbnail.{imageExtension}",
                ContentType = imageFile.ContentType
            };

            using (var image = Image.Load(imageFile.Content))
            {
                image.Mutate(x => x.Resize(0, 48));
                using (var output = new MemoryStream())
                {
                    image.Save(output, image.Metadata.DecodedImageFormat);
                    thumbnail.Content = output.ToArray();
                }
            }
            thumbnail.Length = thumbnail.Content.Length;

            return thumbnail;
        }
    }
}
