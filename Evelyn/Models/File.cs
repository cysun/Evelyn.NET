using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Markdig;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Evelyn.Models
{
    public class File
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string ContentType { get; set; }
        public long Length { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public byte[] Content { get; set; }

        [NotMapped]
        public string Text
        {
            get => Encoding.UTF8.GetString(Content);
            set => Content = Encoding.UTF8.GetBytes(value);
        }

        public System.IO.Stream OpenReadStream()
        {
            return new System.IO.MemoryStream(Content, false);
        }

        public void Append(File another)
        {
            byte[] newContent = new byte[Content.Length + another.Content.Length];
            Buffer.BlockCopy(Content, 0, newContent, 0, Content.Length);
            Buffer.BlockCopy(another.Content, 0, newContent, Content.Length, another.Content.Length);
            Length += another.Length;
            Timestamp = DateTime.Now;
        }

        public static File FromUploadedFile(IFormFile uploadedFile)
        {
            var file = new Models.File
            {
                Name = System.IO.Path.GetFileName(uploadedFile.FileName),
                ContentType = uploadedFile.ContentType,
                Length = uploadedFile.Length
            };

            using (var memoryStream = new System.IO.MemoryStream())
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
            var imageName = System.IO.Path.GetFileNameWithoutExtension(imageFile.Name);
            var imageExtension = System.IO.Path.GetExtension(imageFile.Name);
            var thumbnail = new File
            {
                Name = $"{imageName} - thumbnail.{imageExtension}",
                ContentType = imageFile.ContentType
            };

            IImageFormat imageFormat;
            using (Image<Rgba32> image = Image.Load(imageFile.Content, out imageFormat))
            {
                image.Mutate(x => x.Resize(0, 48));
                using (var output = new System.IO.MemoryStream())
                {
                    image.Save(output, imageFormat);
                    thumbnail.Content = output.ToArray();
                }
            }
            thumbnail.Length = thumbnail.Content.Length;

            return thumbnail;
        }
    }
}
