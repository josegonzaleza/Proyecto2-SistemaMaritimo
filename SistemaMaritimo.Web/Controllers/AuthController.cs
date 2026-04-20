using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _authService.LoginAsync(model);
            var json = JObject.Parse(response);

            bool exito = json["exito"]?.Value<bool>() ?? false;
            var rolesArray = json["roles"] as JArray;
            if (rolesArray != null)
            {
                HttpContext.Session.SetString("Roles", rolesArray.ToString());
            }
            if (!exito)
            {
                ViewBag.Error = json["mensaje"]?.ToString();
                return View(model);
            }

            HttpContext.Session.SetString("Token", json["token"]?.ToString() ?? "");
            HttpContext.Session.SetString("NombreUsuario", json["nombreUsuario"]?.ToString() ?? "");

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}