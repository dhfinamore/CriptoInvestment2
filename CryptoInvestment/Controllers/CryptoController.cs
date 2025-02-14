using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoInvestment.Controllers;

[Authorize]
public class CryptoController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
    
    public IActionResult CustomerConfiguration()
    {
        return View();
    }
}