using Microsoft.EntityFrameworkCore;
using MvcLibrary.Models;

namespace MvcLibrary.Data;

public class UserDBContext : DbContext
{
    public UserDBContext(DbContextOptions<UserDBContext> options): base(options) {
        Database.EnsureCreated();
        Users = Set<UserModel>() as DbSet<UserModel>;
    }

    public DbSet<UserModel> Users { get; set; }
}