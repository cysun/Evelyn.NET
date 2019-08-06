using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evelyn.Models;
using Evelyn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService bookService;
        private readonly FileService fileService;

        public BooksController(BookService bookService, FileService fileService)
        {
            this.bookService = bookService;
            this.fileService = fileService;
        }

        public IActionResult List()
        {
            return View(bookService.GetBooks());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Book());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Book book, IFormFile content, IFormFile cover)
        {
            if (content != null)
                book.MarkdownFile = await fileService.SaveFile(content);

            if (cover != null)
                book.CoverFile = await fileService.SaveFile(cover);

            bookService.AddBook(book);

            return RedirectToAction(nameof(List));
        }
    }
}
