using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CryptoInvestment.Models;

namespace CryptoInvestment.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetString("UserId");

        return string.IsNullOrEmpty(userId) ? RedirectToAction("Login", "Authentication") :
            RedirectToAction("Dashboard", "Crypto");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
