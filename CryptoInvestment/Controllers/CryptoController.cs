using Microsoft.AspNetCore.Mvc;

namespace CryptoInvestment.Controllers;

public class CryptoController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}