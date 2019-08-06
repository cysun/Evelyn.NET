using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Evelyn.Models
{
    public class File
    {
        public int FileId { get; set; }

        public string Name { get; set; }
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
    }
}
