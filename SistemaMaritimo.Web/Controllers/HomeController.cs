using Microsoft.AspNetCore.Mvc;

namespace SistemaMaritimo.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Auth");

            ViewBag.Usuario = HttpContext.Session.GetString("NombreUsuario");
            return View();
        }
    }
}