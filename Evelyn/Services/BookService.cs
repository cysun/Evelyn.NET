using System.Collections.Generic;
using System.Linq;
using Evelyn.Models;
using Microsoft.EntityFrameworkCore;

namespace Evelyn.Services
{
    public class BookService
    {
        private readonly AppDbContext _db;

        public BookService(AppDbContext db)
        {
            _db = db;
        }

        public List<Book> GetBooks()
        {
            return _db.Books.OrderByDescending(b => b.LastUpdated).ToList();
        }

        public List<Book> SearchBooks(string term)
        {
            return _db.Books.Where(b => EF.Functions.Like(b.Title, $"%{term}%")
                || EF.Functions.Like(b.Author, $"%{term}%")).ToList();
        }

        public Book GetBook(int id)
        {
            var book = _db.Books.Where(b => b.Id == id).Include(b => b.Chapters).SingleOrDefault();

            if (book != null && book.Chapters.Count > 1)
                book.Chapters = book.Chapters.OrderBy(c => c.Number).ToList();

            return book;
        }

        public void AddBook(Book book) => _db.Books.Add(book);

        public void DeleteBook(Book book)
        {
            _db.Books.Remove(book);
            _db.SaveChanges();
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
