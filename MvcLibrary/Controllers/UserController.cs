using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Models;
using MvcLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MvcLibrary.Controllers;

[Route("[controller]/")]
public class UserController : Controller
{
    private readonly UserDBContext _db;

    public UserController(UserDBContext context) {
        _db = context;
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

    [Route("login/")]
    public IActionResult Login() {
        SetViewDataFromSession();

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

        String calculatedHash = CryptoCalculator.CreateSHA256WithSalt(passwordGiven, passwordSalt);

        if (calculatedHash != userModel.PasswordHash) {
            // password incorrect
            ViewData["LoginResult"] = "Incorrect login or password";

            return View();
        }

        // user authenticated
        ViewData["LoginResult"] = "Successfully logged in";

        HttpContext.Session.SetString("username", usernameGiven);
        HttpContext.Session.SetString("isadmin", userModel.IsAdmin.ToString());

        SetViewDataFromSession();

        return View();
    }

    [Route("logout/")]
    public IActionResult Logout() {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    [Route("register/")]
    public IActionResult Register() {
        if (HttpContext.Session.GetString("username") != null) {
            return RedirectToAction("Index", "Home");
        }

        SetViewDataFromSession();

        return View();
    }

    [Route("register/")]
    [HttpPost]
    public IActionResult Register(IFormCollection form) {
        if (HttpContext.Session.GetString("username") != null) {
            return RedirectToAction("Index", "Home");
        }

        if (form["username"] == "" || form["password1"] == "" || form["password2"] == "") {
            ViewData["RegisterResult"] = "All fields must be filled";

            return View();
        }

        String usernameGiven = (String ?) form["username"] ?? "";
        String password1Given = (String ?) form["password1"] ?? "";
        String password2Given = (String ?) form["password2"] ?? "";

        if (password1Given != password2Given) {
            ViewData["RegisterResult"] = "Passwords do not match";

            return View();
        }

        // check if user exists
        int foundNumber =
            (
                from user in _db.Users
                where user.Username == usernameGiven
                select user
            ).Count();

        if (foundNumber != 0) {
            ViewData["RegisterResult"] = "User already exists";

            return View();
        }

        // user does not exist

        String passwordSalt = CryptoCalculator.generateRandomString(128);
        String passwordHash = CryptoCalculator.CreateSHA256WithSalt(password1Given, passwordSalt);
        String apiKey = "";

        do {
            apiKey = CryptoCalculator.generateRandomString(256);

            foundNumber =
                (
                    from user in _db.Users
                    where user.APIKey == apiKey
                    select user
                ).Count();
        } while (foundNumber != 0);

        UserModel newUser = new UserModel {
            Username = usernameGiven,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            APIKey = apiKey,
            IsAdmin = false
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();

        ViewData["RegisterResult"] = "Successfully registered";
        ViewData["RegisterSuccess"] = "true";

        SetViewDataFromSession();

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

class CryptoCalculator {
    public static String CreateSHA256(String input) {
        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }

    public static String CreateSHA256WithSalt(String input, String salt) {
        return CreateSHA256(input + salt);
    }

    public static String generateRandomString(int len) {
        Random random = new Random();

        byte[] bytes = new byte[len];
        random.NextBytes(bytes);

        return Convert.ToHexString(bytes);
    }
}