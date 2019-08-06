using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return db.Books.Where(b => b.BookId == bookId).Include(b => b.Chapters).SingleOrDefault();
        }

        public Book AddBook(Book book)
        {
            db.Books.Add(book);
            db.SaveChanges();
            return book;
        }

        public void SaveChanges() => db.SaveChanges();
    }
}
