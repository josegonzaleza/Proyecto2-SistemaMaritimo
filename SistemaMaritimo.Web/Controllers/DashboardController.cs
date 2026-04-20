using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [SessionAuthorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _service;

        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.ObtenerDashboardAsync();
            if (model == null)
            {
                TempData["Error"] = "No se pudo cargar la información del dashboard.";
                return View(new Models.DashboardViewModel());
            }

            return View(model);
        }
    }
}