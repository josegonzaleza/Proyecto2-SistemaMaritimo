using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [RoleAuthorize("Administrador", "Capitan", "PrimerOficial", "IngenieroJefe")]
    public class BarcosController : Controller
    {
        private readonly BarcosService _service;

        public BarcosController(BarcosService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.ObtenerTodosAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new BarcoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BarcoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.CrearAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                return View(model);
            }

            TempData["Success"] = "Barco creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BarcoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.EditarAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                return View(model);
            }

            TempData["Success"] = "Barco actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.ArchivarAsync(id);
            TempData["Success"] = "Barco archivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activar(int id)
        {
            await _service.ActivarAsync(id);
            TempData["Success"] = "Barco activado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}