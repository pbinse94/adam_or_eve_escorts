using Microsoft.AspNetCore.Mvc;

namespace WebAdmin.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error401()
        {
            return View();
        }
        public IActionResult Error500()
        {
            return View();
        }
    }
}
