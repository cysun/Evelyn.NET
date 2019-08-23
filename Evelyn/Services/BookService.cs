using System.Collections.Generic;
using System.Linq;
using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class BookService
    {
        private readonly AppDbContext db;

        public BookService(AppDbContext db)
        {
            this.db = db;
        }

        public List<Book> GetBooks()
        {
            return db.Books.OrderByDescending(b => b.LastUpdated).ToList();
        }

        public Book GetBook(int bookId)
        {
            var book = db.Books.Where(b => b.BookId == bookId).Include(b => b.Chapters).SingleOrDefault();

	        if (book != null && book.Chapters.Count > 1)
                book.Chapters = book.Chapters.OrderBy(c => c.Number).ToList();

            return book;
        }

        public void AddBook(Book book)
        {
            db.Books.Add(book);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
