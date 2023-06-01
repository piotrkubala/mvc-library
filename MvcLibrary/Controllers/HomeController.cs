using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers;

public class HomeController : Controller
{
    private void SetViewDataFromSession() {
        if (HttpContext.Session.GetString("username") == null) {
            ViewData["Username"] = "";
            ViewData["IsAdmin"] = "";

            return;
        }

        ViewData["Username"] = HttpContext.Session.GetString("username");
        ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");
    }

    public HomeController() {
    }

    public IActionResult Index() {
        SetViewDataFromSession();

        return View();
    }

    public IActionResult Privacy() {
        SetViewDataFromSession();

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
