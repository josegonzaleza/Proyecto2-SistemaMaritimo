using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Helpers;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly AuthRepository _authRepository;

        public UsuariosController(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_authRepository.ObtenerUsuarios());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usuario = _authRepository.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            var rolId = _authRepository.ObtenerRolIdActualUsuario(id);

            return Ok(new
            {
                usuario.Id,
                usuario.NombreUsuario,
                usuario.Activo,
                RolId = rolId
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] UsuarioCreateRequest request)
        {
            var existente = _authRepository.ObtenerUsuarioPorNombre(request.NombreUsuario);
            if (existente != null)
            {
                return BadRequest(new { mensaje = "El nombre de usuario ya existe." });
            }

            var claveHash = PasswordHelper.HashPassword(request.Clave);
            _authRepository.CrearUsuario(request.NombreUsuario, claveHash, request.RolId);

            return Ok(new { mensaje = "Usuario creado correctamente" });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UsuarioEditRequest request)
        {
            var existente = _authRepository.ObtenerUsuarioPorNombre(request.NombreUsuario);
            if (existente != null && existente.Id != id)
            {
                return BadRequest(new { mensaje = "El nombre de usuario ya existe." });
            }

            var realizadoPor = User.Identity?.Name ?? "sistema";

            _authRepository.EditarUsuario(id, request.NombreUsuario, request.RolId, request.Activo, realizadoPor);
            return Ok(new { mensaje = "Usuario actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var usuarioActual = User.Identity?.Name ?? "";
            var usuario = _authRepository.ObtenerUsuarioPorId(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            if (usuario.NombreUsuario.Equals(usuarioActual, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { mensaje = "No puede desactivar el usuario que está en sesión." });
            }

            _authRepository.EliminarUsuario(id);
            return Ok(new { mensaje = "Usuario desactivado correctamente" });
        }

        [HttpPut("{id}/activar")]
        public IActionResult Activar(int id)
        {
            _authRepository.ActivarUsuario(id);
            return Ok(new { mensaje = "Usuario activado correctamente" });
        }

        [HttpPut("{id}/rol")]
        public IActionResult CambiarRol(int id, [FromBody] CambiarRolRequest request)
        {
            _authRepository.CambiarRolUsuario(id, request.RolId, request.RealizadoPor);
            return Ok(new { mensaje = "Rol actualizado correctamente" });
        }

        [HttpPut("{id}/password")]
        public IActionResult CambiarPassword(int id, [FromBody] CambiarPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NuevaClave))
            {
                return BadRequest(new { mensaje = "La nueva contraseña es obligatoria." });
            }

            var hash = PasswordHelper.HashPassword(request.NuevaClave);
            _authRepository.CambiarPassword(id, hash);

            return Ok(new { mensaje = "Contraseña actualizada correctamente" });
        }

        [HttpGet("bitacora")]
        public IActionResult Bitacora()
        {
            return Ok(_authRepository.ObtenerBitacoraCambiosRoles());
        }
    }
}