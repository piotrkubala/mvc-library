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

        ViewData["BooksList"] = books;

        return View();
    }

    [Route("addbook/")]
    public IActionResult AddBook() {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        BookCategoryModel []bookCategories = _db.BookCategories.ToArray();

        ViewData["BookCategories"] = bookCategories;

        return View();
    }

    [Route("addbook/")]
    [HttpPost]
    public IActionResult AddBook(IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        List<String> fields = new List<String> {
            "title",
            "author",
            "description",
            "publisher",
            "isbn",
            "publication-date",
            "book-category-id"
        };

        foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["AddBookMessage"] = "All fields are required";

                return View();
            }
        }

        try {
            List<BookCategoryModel> category = (
                from bookCategory in _db.BookCategories where bookCategory.BookCategoryId == Int32.Parse((String ?) form["book-category-id"]) select bookCategory
            ).ToList();

            if (category.Count() == 0) {
                ViewData["AddBookMessage"] = "Invalid category";

                return View();
            }

            _db.Books.Add(new BookModel {
                Title = ((String ?) form["title"]) ?? "",
                Author = ((String ?) form["author"]) ?? "",
                Publisher = ((String ?) form["publisher"]) ?? "",
                Description = ((String ?) form["description"]) ?? "",
                Isbn = ((String ?) form["isbn"]) ?? "",
                PublicationDate = DateTime.Parse((String ?) form["publication-date"]),
                BookCategoryId = Int32.Parse((String ?) form["book-category-id"])
            });

            _db.SaveChanges();
        } catch (Exception) {
            ViewData["AddBookMessage"] = "Invalid field format";

            return View();
        }

        ViewData["AddBookMessage"] = "Book added successfully";

        return RedirectToAction("AddBook");
    }

    [Route("addcategory/")]
    public IActionResult AddCategory() {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        return View();
    }

    [Route("addcategory/")]
    [HttpPost]
    public IActionResult AddCategory(IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        if (((String ?) form["category-name"]) is null || form["category-name"] == "") {
            ViewData["AddBookMessage"] = "Name is required";

            return View();
        }

        if ((from bookCategory in _db.BookCategories where bookCategory.Name == ((String ?) form["category-name"]) select bookCategory).Count() > 0) {
            ViewData["AddBookMessage"] = "Category already exists";

            return View();
        }

        _db.BookCategories.Add(new BookCategoryModel {
            Name = ((String ?) form["category-name"]) ?? "",
            Description = ((String ?) form["category-description"]) ?? ""
        });

        _db.SaveChanges();

        ViewData["AddBookMessage"] = "Category added successfully";

        return View();
    }

    [Route("editbook/{bookId:int}")]
    public IActionResult EditBook(int bookId) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        BookModel ?book = (
            from bookModel in _db.Books where bookModel.BookId == bookId select bookModel
        ).FirstOrDefault();

        if (book is null) {
            return RedirectToAction("ListBooks");
        }

        BookCategoryModel []bookCategories = _db.BookCategories.ToArray();

        ViewData["BookCategories"] = bookCategories;

        ViewData["book-id"] = book.BookId;
        ViewData["title"] = book.Title;
        ViewData["author"] = book.Author;
        ViewData["description"] = book.Description;
        ViewData["publisher"] = book.Publisher;
        ViewData["isbn"] = book.Isbn;
        ViewData["publication-date"] = book.PublicationDate?.ToString("yyyy-MM-dd");
        ViewData["book-category-id"] = book.BookCategoryId;

        return View();
    }

    [Route("editbook/{bookId:int}")]
    [HttpPost]
    public IActionResult EditBook(int bookId, IFormCollection form) {
        if (HttpContext.Session.GetString("username") == null || HttpContext.Session.GetString("username") == "") {
            return RedirectToAction("Login", "User");
        }

        SetViewDataFromSession();

        BookModel ?book = (
            from bookModel in _db.Books where bookModel.BookId == bookId select bookModel
        ).FirstOrDefault();

        if (book is null) {
            return RedirectToAction("ListBooks");
        }

        List<String> fields = new List<String> {
            "title",
            "author",
            "description",
            "publisher",
            "isbn",
            "publication-date",
            "book-category-id"
        };

        foreach (String field in fields) {
            if (((String ?) form[field]) is null || (String ?) form[field] == "") {
                ViewData["EditBookMessage"] = "All fields are required";

                return View();
            }
        }

        try {
            List<BookCategoryModel> category = (
                from bookCategory in _db.BookCategories where bookCategory.BookCategoryId == Int32.Parse((String ?) form["book-category-id"]) select bookCategory
            ).ToList();

            if (category.Count() == 0) {
                ViewData["EditBookMessage"] = "Invalid category";

                return View();
            }

            book.Title = ((String ?) form["title"]) ?? "";
            book.Author = ((String ?) form["author"]) ?? "";
            book.Publisher = ((String ?) form["publisher"]) ?? "";
            book.Description = ((String ?) form["description"]) ?? "";
            book.Isbn = ((String ?) form["isbn"]) ?? "";
            book.PublicationDate = DateTime.Parse((String ?) form["publication-date"]);
            book.BookCategoryId = Int32.Parse((String ?) form["book-category-id"]);

            System.Console.WriteLine(form["book-category-id"]);

            _db.Books.Update(book);

            _db.SaveChanges();
        } catch (Exception) {
            ViewData["EditBookMessage"] = "Invalid field format";

            return View();
        }

        ViewData["EditBookMessage"] = "Book edited successfully";

        return RedirectToAction("EditBook", bookId);
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