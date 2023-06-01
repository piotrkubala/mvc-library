using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Models;
using MvcLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MvcLibrary.Controllers;

public struct UserData {
    public int UserId;
    public String Username;
    public bool IsApproved;
    public bool IsAdmin;
};

[Route("[controller]/")]
public class UserController : Controller
{
    private readonly LibraryDBContext _db;

    public UserController(LibraryDBContext context) {
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
        if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "") {
            return RedirectToAction("Index", "Home");
        }

        SetViewDataFromSession();

        return View();
    }

    [Route("login/")]
    [HttpPost]
    public IActionResult Login(IFormCollection form) {
        if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "") {
            return RedirectToAction("Index", "Home");
        }

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

        if (userModel.IsApproved == false) {
            // user not approved
            ViewData["LoginResult"] = "Your account is not approved yet";

            return View();
        }

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
        HttpContext.Session.SetInt32("userid", userModel.UserId);

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
        if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "") {
            return RedirectToAction("Index", "Home");
        }

        SetViewDataFromSession();

        return View();
    }

    [Route("register/")]
    [HttpPost]
    public IActionResult Register(IFormCollection form) {
        if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("username") != "") {
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

        String passwordSalt = CryptoCalculator.GenerateRandomString(128);
        String passwordHash = CryptoCalculator.CreateSHA256WithSalt(password1Given, passwordSalt);
        String apiKey = "";

        do {
            apiKey = CryptoCalculator.GenerateRandomString(256);

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
            IsAdmin = false,
            IsApproved = false
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();

        ViewData["RegisterResult"] = "Successfully registered, waiting for admin to approve your account";
        ViewData["RegisterSuccess"] = "true";

        SetViewDataFromSession();

        return View();
    }

    [Route("manage/")]
    public IActionResult ManageUsers() {
        if (HttpContext.Session.GetString("username") == null) {
            return RedirectToAction("Index", "Home");
        }

        if (HttpContext.Session.GetString("isadmin") != "True") {
            return RedirectToAction("Index", "Home");
        }

        SetViewDataFromSession();

        List<UserData> allUsers =
            (
                from user in _db.Users
                orderby user.IsAdmin descending, user.IsApproved ascending, user.Username ascending
                select new UserData {
                    UserId = user.UserId,
                    Username = user.Username,
                    IsApproved = user.IsApproved,
                    IsAdmin = user.IsAdmin
                }
            ).ToList();

        ViewData["AllUsers"] = allUsers;

        return View();
    }

    [Route("manage/")]
    [HttpPost]
    public IActionResult ManageUsers(IFormCollection form) {
        String ?username = HttpContext.Session.GetString("username");
        int ?userId = HttpContext.Session.GetInt32("userid");

        if (username is null) {
            return RedirectToAction("Index", "Home");
        }

        if (HttpContext.Session.GetString("isadmin") != "True") {
            return RedirectToAction("Index", "Home");
        }

        List<UserData> allUsersNotAdmin =
            (
                from user in _db.Users
                where !user.IsAdmin || user.Username == username
                select new UserData {
                    UserId = user.UserId,
                    Username = user.Username,
                    IsApproved = user.IsApproved,
                    IsAdmin = user.IsAdmin
                }
            ).ToList();

        for (int i = 0; i < allUsersNotAdmin.Count; i++) {
            UserData user = allUsersNotAdmin[i];

            if (user.Username != username && user.UserId == userId && user.IsAdmin) {
                continue;
            }

            String adminKey = $"admin-{user.Username}-{user.UserId}";
            String approveKey = $"approved-{user.Username}-{user.UserId}";

            if (form.ContainsKey(adminKey) && form[adminKey] == "on") {
                user.IsAdmin = true;
                user.IsApproved = true;
            } else if (user.Username == username) {
                user.IsAdmin = form[adminKey] == "on";
                HttpContext.Session.SetString("isadmin", user.IsAdmin ? "True" : "False");
            } else {
                user.IsApproved = form[approveKey] == "on";
            }

            UserModel ?userModel = _db.Users.Find(user.UserId);

            if (userModel is null) {
                continue;
            }

            userModel.IsAdmin = user.IsAdmin;
            userModel.IsApproved = user.IsApproved;

            _db.Users.Update(userModel);
        }

        _db.SaveChanges();

        return RedirectToAction("ManageUsers");
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

public class CryptoCalculator {
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

    public static String GenerateRandomString(int len) {
        Random random = new Random();

        byte[] bytes = new byte[len];
        random.NextBytes(bytes);

        return Convert.ToHexString(bytes);
    }
}