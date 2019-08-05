using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Name);
            modelBuilder.Entity<File>().HasDiscriminator<string>("Type");
            modelBuilder.Entity<File>().Property(f => f.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Book>().Property(b => b.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<Chapter>().HasKey(c => new { c.BookId, c.Number });
            modelBuilder.Entity<Bookmark>().Property(b => b.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
