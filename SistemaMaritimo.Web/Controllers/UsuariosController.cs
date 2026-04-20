using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using SistemaMaritimo.Web.Filters;
using SistemaMaritimo.Web.Models;
using SistemaMaritimo.Web.Services;

namespace SistemaMaritimo.Web.Controllers
{
    [RoleAuthorize("Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UsuariosService _usuariosService;

        public UsuariosController(UsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuariosService.ObtenerUsuariosAsync();
            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _usuariosService.ObtenerRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _usuariosService.ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Nombre");
                return View(model);
            }

            var result = await _usuariosService.CrearUsuarioAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                var roles = await _usuariosService.ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Nombre");
                return View(model);
            }

            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _usuariosService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null) return RedirectToAction(nameof(Index));

            var roles = await _usuariosService.ObtenerRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Nombre", usuario.RolId);

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _usuariosService.ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Nombre", model.RolId);
                return View(model);
            }

            var result = await _usuariosService.EditarUsuarioAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                var roles = await _usuariosService.ObtenerRolesAsync();
                ViewBag.Roles = new SelectList(roles, "Id", "Nombre", model.RolId);
                return View(model);
            }

            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CambiarRol(int id)
        {
            var roles = await _usuariosService.ObtenerRolesAsync();
            ViewBag.UsuarioId = id;
            ViewBag.Roles = new SelectList(roles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CambiarRol(int usuarioId, int rolId)
        {
            var realizadoPor = HttpContext.Session.GetString("NombreUsuario") ?? "sistema";
            var ok = await _usuariosService.CambiarRolAsync(usuarioId, rolId, realizadoPor);

            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo cambiar el rol.");
                var roles = await _usuariosService.ObtenerRolesAsync();
                ViewBag.UsuarioId = usuarioId;
                ViewBag.Roles = new SelectList(roles, "Id", "Nombre");
                return View();
            }

            TempData["Success"] = "Rol actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CambiarPassword(int id)
        {
            return View(new CambiarPasswordViewModel { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _usuariosService.CambiarPasswordAsync(model);

            if (!result.ok)
            {
                ViewBag.Error = result.mensaje;
                return View(model);
            }

            TempData["Success"] = "Contraseña actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var nombreUsuarioActual = HttpContext.Session.GetString("NombreUsuario") ?? "";
            var usuarios = await _usuariosService.ObtenerUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(x => x.Id == id);

            if (usuario != null && usuario.NombreUsuario.Equals(nombreUsuarioActual, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "No puede desactivar el usuario que está en sesión.";
                return RedirectToAction(nameof(Index));
            }

            await _usuariosService.EliminarUsuarioAsync(id);
            TempData["Success"] = "Usuario desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activar(int id)
        {
            await _usuariosService.ActivarUsuarioAsync(id);
            TempData["Success"] = "Usuario activado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Bitacora()
        {
            var data = await _usuariosService.ObtenerBitacoraAsync();
            return View(data);
        }
    }
}