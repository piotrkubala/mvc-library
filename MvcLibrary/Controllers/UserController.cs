using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Models;
using MvcLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcLibrary.Controllers;

[Route("[controller]/")]
public class UserController : Controller
{
    private readonly UserDBContext _db;

    public UserController(UserDBContext context) {
        _db = context;
    }

    [Route("login/")]
    public IActionResult Login() {
        return View();
    }

    [Route("login/")]
    [HttpPost]
    public IActionResult Login(IFormCollection form) {
        // check if all fields are filled
        if (form["username"] == "" || form["password"] == "") {
            ViewData["LoginResult"] = "Both fields must be filled";

            return View();
        }

        String usernameGiven = (String ?) form["username"] ?? "";
        String passwordGiven = (String ?) form["password"] ?? "";

        // check if user exists

        var found =
            (
                from user in _db.Users
                where user.Username == usernameGiven
                select user
            );

        if (found.Count() == 0) {
            // user not found
            ViewData["LoginResult"] = "Incorrect login or password";

            return View();
        }

        // user found
        UserModel userModel = found.First();
        String passwordSalt = userModel.PasswordSalt;

        String calculatedHash = HashCalculator.CreateSHA256WithSalt(passwordGiven, passwordSalt);

        if (calculatedHash != userModel.PasswordHash) {
            // password incorrect
            ViewData["LoginResult"] = "Incorrect login or password";

            return View();
        }

        // user authenticated
        ViewBag.Username = usernameGiven;
        ViewBag.IsAdmin = userModel.IsAdmin;
        ViewData["LoginResult"] = "Successfully logged in";

        HttpContext.Session.SetString("username", usernameGiven);
        HttpContext.Session.SetString("isadmin", userModel.IsAdmin.ToString());

        return View();
    }

    [Route("logout/")]
    public IActionResult Logout() {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    [Route("register/")]
    public IActionResult Register() {
        return View();
    }

    [Route("register/")]
    [HttpPost]
    public IActionResult Register(IFormCollection form) {
        return View();
    }

    [HttpGet("{*url}", Order = 999)]
    public IActionResult CatchAll() {
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

class HashCalculator {
    public static string CreateSHA256(String input) {
        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }

    public static string CreateSHA256WithSalt(String input, String salt) {
        return CreateSHA256(input + salt);
    }
}