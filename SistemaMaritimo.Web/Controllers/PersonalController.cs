using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [RoleAuthorize("Administrador", "Capitan", "PrimerOficial", "IngenieroJefe")]
    public class PersonalController : Controller
    {
        private readonly PersonalService _service;

        public PersonalController(PersonalService service)
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
            CargarRolesPrimarios();
            return View(new PersonalViewModel { FechaContratacion = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CargarRolesPrimarios();
                return View(model);
            }

            var result = await _service.CrearAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                CargarRolesPrimarios();
                return View(model);
            }

            TempData["Success"] = "Personal creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            CargarRolesPrimarios();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PersonalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CargarRolesPrimarios();
                return View(model);
            }

            var result = await _service.EditarAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                CargarRolesPrimarios();
                return View(model);
            }

            TempData["Success"] = "Personal actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DesactivarAsync(id);
            TempData["Success"] = "Registro desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var detalle = await _service.ObtenerDetalleAsync(id);
            if (detalle == null) return RedirectToAction(nameof(Index));

            return View(detalle);
        }

        [HttpGet]
        public IActionResult CrearLicencia(int personalId)
        {
            return View(new LicenciaViewModel
            {
                PersonalId = personalId,
                FechaVencimiento = DateTime.Today.AddMonths(6)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CrearLicencia(LicenciaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _service.CrearLicenciaAsync(model);

            if (!ok)
            {
                ViewBag.Error = "No se pudo registrar la licencia.";
                return View(model);
            }

            TempData["Success"] = "Licencia registrada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = model.PersonalId });
        }

        public async Task<IActionResult> EliminarLicencia(int personalId, int licenciaId)
        {
            await _service.EliminarLicenciaAsync(personalId, licenciaId);
            TempData["Success"] = "Licencia eliminada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = personalId });
        }

        [HttpGet]
        public async Task<IActionResult> CrearAsignacion(int personalId)
        {
            var barcos = await _service.ObtenerBarcosAsync();
            ViewBag.Barcos = new SelectList(barcos, "Id", "NombreEmbarcacion");

            return View(new AsignacionTripulacionViewModel
            {
                PersonalId = personalId,
                FechaInicio = DateTime.Today
            });
        }

        [HttpPost]
        public async Task<IActionResult> CrearAsignacion(AsignacionTripulacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var barcos = await _service.ObtenerBarcosAsync();
                ViewBag.Barcos = new SelectList(barcos, "Id", "NombreEmbarcacion");
                return View(model);
            }

            var ok = await _service.CrearAsignacionAsync(model);

            if (!ok)
            {
                ViewBag.Error = "No se pudo registrar la asignación.";
                var barcos = await _service.ObtenerBarcosAsync();
                ViewBag.Barcos = new SelectList(barcos, "Id", "NombreEmbarcacion");
                return View(model);
            }

            TempData["Success"] = "Asignación registrada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = model.PersonalId });
        }

        private void CargarRolesPrimarios()
        {
            ViewBag.RolesPrimarios = new List<SelectListItem>
            {
                new SelectListItem { Value = "Capitan", Text = "Capitán" },
                new SelectListItem { Value = "PrimerOficial", Text = "Primer Oficial" },
                new SelectListItem { Value = "Ingeniero", Text = "Ingeniero" },
                new SelectListItem { Value = "Marinero", Text = "Marinero" },
                new SelectListItem { Value = "PersonalBase", Text = "Personal de Base" }
            };
        }
    }
}