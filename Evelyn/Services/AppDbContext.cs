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
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Name);
            modelBuilder.Entity<File>().Property(f => f.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Book>().Property(b => b.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Book>().Property(b => b.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<Chapter>().HasAlternateKey(c => new { c.BookId, c.Number });
            modelBuilder.Entity<Chapter>().Property(c => c.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Bookmark>().Property(b => b.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Bookmark>().Property(b => b.IsManual).HasDefaultValue(false);
        }
    }
}
