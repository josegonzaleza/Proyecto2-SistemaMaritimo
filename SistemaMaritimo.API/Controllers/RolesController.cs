using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RolesController : ControllerBase
    {
        private readonly AuthRepository _authRepository;

        public RolesController(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_authRepository.ObtenerRoles());
        }

        [HttpPost]
        public IActionResult Post([FromBody] RolCreateRequest request)
        {
            _authRepository.CrearRol(request.Nombre);
            return Ok(new { mensaje = "Rol creado correctamente" });
        }
    }
}