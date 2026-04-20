using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [RoleAuthorize("Administrador", "Capitan", "PrimerOficial")]
    public class TravesiasController : Controller
    {
        private readonly TravesiasService _service;
        private readonly BarcosService _barcosService;

        public TravesiasController(TravesiasService service, BarcosService barcosService)
        {
            _service = service;
            _barcosService = barcosService;
        }

        public async Task<IActionResult> Index(string? barcoId, string? puertoDestino)
        {
            var data = await _service.ObtenerTodasAsync(barcoId, puertoDestino);
            var barcos = await _barcosService.ObtenerTodosAsync();

            ViewBag.Barcos = new SelectList(barcos.Where(x => !x.Archivado), "Id", "NombreEmbarcacion", barcoId);
            ViewBag.PuertoDestino = puertoDestino;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarBarcosAsync();
            return View(new TravesiaViewModel
            {
                FechaPrevistaSalida = DateTime.Now,
                FechaPrevistaLlegada = DateTime.Now.AddDays(1),
                Estado = "Planeada"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TravesiaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarBarcosAsync(model.BarcoId);
                return View(model);
            }

            var result = await _service.CrearAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                await CargarBarcosAsync(model.BarcoId);
                return View(model);
            }

            TempData["Success"] = "Travesía creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            await CargarBarcosAsync(model.BarcoId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TravesiaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarBarcosAsync(model.BarcoId);
                return View(model);
            }

            var result = await _service.EditarAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                await CargarBarcosAsync(model.BarcoId);
                return View(model);
            }

            TempData["Success"] = "Travesía actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            var barcos = await _barcosService.ObtenerTodosAsync();
            ViewBag.NombreBarco = barcos.FirstOrDefault(x => x.Id == model.BarcoId)?.NombreEmbarcacion ?? "";

            return View(model);
        }

        [HttpGet]
        public IActionResult CambiarEstado(int id)
        {
            ViewBag.TravesiaId = id;
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Planeada", Text = "Planeada" },
                new SelectListItem { Value = "En Curso", Text = "En Curso" },
                new SelectListItem { Value = "Completada", Text = "Completada" },
                new SelectListItem { Value = "Cancelada", Text = "Cancelada" }
            };

            return View(new CambiarEstadoTravesiaViewModel { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(CambiarEstadoTravesiaViewModel model)
        {
            var usuario = HttpContext.Session.GetString("NombreUsuario") ?? "sistema";
            var result = await _service.CambiarEstadoAsync(model.Id, model.NuevoEstado, usuario);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                ViewBag.TravesiaId = model.Id;
                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Planeada", Text = "Planeada" },
                    new SelectListItem { Value = "En Curso", Text = "En Curso" },
                    new SelectListItem { Value = "Completada", Text = "Completada" },
                    new SelectListItem { Value = "Cancelada", Text = "Cancelada" }
                };
                return View(model);
            }

            TempData["Success"] = "Estado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarBarcosAsync(int? seleccionado = null)
        {
            var barcos = await _barcosService.ObtenerTodosAsync();
            ViewBag.Barcos = new SelectList(barcos.Where(x => !x.Archivado), "Id", "NombreEmbarcacion", seleccionado);
        }
    }
}