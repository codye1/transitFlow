using Microsoft.AspNetCore.Mvc;

namespace TransitFlow.mvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Auth.cshtml");
        }
    }
}