using Microsoft.EntityFrameworkCore;
using MvcLibrary.Models;

namespace MvcLibrary.Data;

public class LibraryDBContext : DbContext {
    public LibraryDBContext(DbContextOptions<LibraryDBContext> options): base(options) {
        Database.EnsureCreated();

        Users = Set<UserModel>() as DbSet<UserModel>;
        Books = Set<BookModel>() as DbSet<BookModel>;
        BookCategories = Set<BookCategoryModel>() as DbSet<BookCategoryModel>;
        BookCopies = Set<BookCopyModel>() as DbSet<BookCopyModel>;
        Borrowings = Set<BorrowingModel>() as DbSet<BorrowingModel>;
        Readers = Set<ReaderModel>() as DbSet<ReaderModel>;
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<BookModel> Books { get; set; }
    public DbSet<BookCategoryModel> BookCategories { get; set; }
    public DbSet<BookCopyModel> BookCopies { get; set; }
    public DbSet<BorrowingModel> Borrowings { get; set; }
    public DbSet<ReaderModel> Readers { get; set; }
}