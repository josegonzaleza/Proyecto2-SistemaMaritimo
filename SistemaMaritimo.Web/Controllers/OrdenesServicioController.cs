using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [RoleAuthorize("Administrador", "IngenieroJefe", "PersonalBase")]
    public class OrdenesServicioController : Controller
    {
        private readonly OrdenesServicioService _service;
        private readonly BarcosService _barcosService;

        public OrdenesServicioController(OrdenesServicioService service, BarcosService barcosService)
        {
            _service = service;
            _barcosService = barcosService;
        }

        public async Task<IActionResult> Index(string? barcoId, string? tecnicoId)
        {
            var data = await _service.ObtenerTodasAsync(barcoId, tecnicoId);
            var barcos = await _barcosService.ObtenerTodosAsync();
            var tecnicos = await _service.ObtenerTecnicosAsync();

            ViewBag.Barcos = new SelectList(barcos.Where(x => !x.Archivado), "Id", "NombreEmbarcacion", barcoId);
            ViewBag.Tecnicos = new SelectList(tecnicos, "Id", "NombreCompleto", tecnicoId);

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarBarcosAsync();
            CargarCombos();
            return View(new OrdenServicioViewModel { FechaLimite = DateTime.Today.AddDays(7) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrdenServicioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarBarcosAsync(model.BarcoId);
                CargarCombos();
                return View(model);
            }

            var result = await _service.CrearAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                await CargarBarcosAsync(model.BarcoId);
                CargarCombos();
                return View(model);
            }

            TempData["Success"] = "Orden creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.ObtenerPorIdAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            await CargarBarcosAsync(model.BarcoId);
            CargarCombos(model.TipoMantenimiento, model.Prioridad);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrdenServicioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarBarcosAsync(model.BarcoId);
                CargarCombos(model.TipoMantenimiento, model.Prioridad);
                return View(model);
            }

            var result = await _service.EditarAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                await CargarBarcosAsync(model.BarcoId);
                CargarCombos(model.TipoMantenimiento, model.Prioridad);
                return View(model);
            }

            TempData["Success"] = "Orden actualizada correctamente.";
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
        public async Task<IActionResult> AsignarTecnico(int id)
        {
            var tecnicos = await _service.ObtenerTecnicosAsync();
            ViewBag.Tecnicos = new SelectList(tecnicos, "Id", "NombreCompleto");

            return View(new AsignarTecnicoViewModel { OrdenId = id });
        }

        [HttpPost]
        public async Task<IActionResult> AsignarTecnico(AsignarTecnicoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tecnicos = await _service.ObtenerTecnicosAsync();
                ViewBag.Tecnicos = new SelectList(tecnicos, "Id", "NombreCompleto");
                return View(model);
            }

            var result = await _service.AsignarTecnicoAsync(model.OrdenId, model.PersonalId);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                var tecnicos = await _service.ObtenerTecnicosAsync();
                ViewBag.Tecnicos = new SelectList(tecnicos, "Id", "NombreCompleto");
                return View(model);
            }

            TempData["Success"] = "Técnico asignado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CambiarEstado(int id)
        {
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Asignada", Text = "Asignada" },
                new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                new SelectListItem { Value = "Pendiente de Aprobación", Text = "Pendiente de Aprobación" }
            };

            return View(new CambiarEstadoOrdenViewModel { OrdenId = id });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(CambiarEstadoOrdenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Asignada", Text = "Asignada" },
                    new SelectListItem { Value = "En Progreso", Text = "En Progreso" },
                    new SelectListItem { Value = "Pendiente de Aprobación", Text = "Pendiente de Aprobación" }
                };
                return View(model);
            }

            var result = await _service.CambiarEstadoAsync(model.OrdenId, model.NuevoEstado);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                return View(model);
            }

            TempData["Success"] = "Estado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Cerrar(int id)
        {
            return View(new CerrarOrdenViewModel { OrdenId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Cerrar(CerrarOrdenViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = HttpContext.Session.GetString("NombreUsuario") ?? "sistema";
            var result = await _service.CerrarOrdenAsync(model.OrdenId, model.InformeCierre, usuario);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                return View(model);
            }

            TempData["Success"] = "Orden cerrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Historial(int barcoId)
        {
            var historial = await _service.ObtenerHistorialAsync(barcoId);
            var barcos = await _barcosService.ObtenerTodosAsync();
            ViewBag.NombreBarco = barcos.FirstOrDefault(x => x.Id == barcoId)?.NombreEmbarcacion ?? "";

            return View(historial);
        }

        private async Task CargarBarcosAsync(int? seleccionado = null)
        {
            var barcos = await _barcosService.ObtenerTodosAsync();
            ViewBag.Barcos = new SelectList(barcos.Where(x => !x.Archivado), "Id", "NombreEmbarcacion", seleccionado);
        }

        private void CargarCombos(string? tipoSeleccionado = null, string? prioridadSeleccionada = null)
        {
            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Preventivo", Text = "Preventivo", Selected = tipoSeleccionado == "Preventivo" },
                new SelectListItem { Value = "Correctivo", Text = "Correctivo", Selected = tipoSeleccionado == "Correctivo" }
            };

            ViewBag.Prioridades = new List<SelectListItem>
            {
                new SelectListItem { Value = "Alta", Text = "Alta", Selected = prioridadSeleccionada == "Alta" },
                new SelectListItem { Value = "Media", Text = "Media", Selected = prioridadSeleccionada == "Media" },
                new SelectListItem { Value = "Baja", Text = "Baja", Selected = prioridadSeleccionada == "Baja" }
            };
        }
    }
}