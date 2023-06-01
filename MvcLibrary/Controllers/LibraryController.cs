using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Models;
using MvcLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MvcLibrary.Controllers;

public struct BookDisplayElement {
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public string Isbn { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string BookCategory { get; set; }
    public int BookCategoryId { get; set; }
}

[Route("[controller]/")]
public class LibraryController : Controller {
    private readonly LibraryDBContext _db;

    public LibraryController(LibraryDBContext contextUser) {
        _db = contextUser;
    }

    private void SetViewDataFromSession() {
        if (HttpContext.Session.GetString("username") == null) {
            ViewData["Username"] = "";
            ViewData["IsAdmin"] = "";

            return;
        }

        ViewData["Username"] = HttpContext.Session.GetString("username");
        ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
    }

    [Route("listbooks/")]
    public IActionResult ListBooks() {
            if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        BookDisplayElement []books = 
            (
                from book in _db.Books join
                bookCategory in _db.BookCategories on book.BookCategoryId equals bookCategory.BookCategoryId
                select new BookDisplayElement {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    Isbn = book.Isbn,
                    PublicationDate = book.PublicationDate,
                    BookCategory = bookCategory.Name,
                    BookCategoryId = bookCategory.BookCategoryId
                }
            ).ToArray();

        ViewData["Books"] = books;

        return View();
    }

    [HttpGet("{*url}", Order = 999)]
    public IActionResult CatchAll() {
        SetViewDataFromSession();

        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}