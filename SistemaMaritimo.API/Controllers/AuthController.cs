using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Helpers;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AuthController(AuthRepository authRepository, JwtHelper jwtHelper, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _authRepository.ObtenerUsuarioPorNombre(request.NombreUsuario);

            if (usuario == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Usuario o contraseña incorrectos."
                });
            }

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.Now)
            {
                return Unauthorized(new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Usuario bloqueado temporalmente. Intente más tarde."
                });
            }

            var claveHash = PasswordHelper.HashPassword(request.Clave);

            if (usuario.ClaveHash != claveHash)
            {
                int maxAttempts = int.Parse(_configuration["SecuritySettings:MaxFailedAttempts"]!);
                int lockMinutes = int.Parse(_configuration["SecuritySettings:LockMinutes"]!);

                int nuevosIntentos = usuario.IntentosFallidos + 1;
                DateTime? bloqueo = null;

                if (nuevosIntentos >= maxAttempts)
                {
                    bloqueo = DateTime.Now.AddMinutes(lockMinutes);
                }

                _authRepository.ActualizarIntentosFallidos(usuario.Id, nuevosIntentos, bloqueo);

                return Unauthorized(new LoginResponse
                {
                    Exito = false,
                    Mensaje = "Usuario o contraseña incorrectos."
                });
            }

            _authRepository.ResetearIntentos(usuario.Id);

            var roles = _authRepository.ObtenerRolesUsuario(usuario.Id);
            var token = _jwtHelper.GenerateToken(usuario.NombreUsuario, roles);

            return Ok(new LoginResponse
            {
                Exito = true,
                Mensaje = "Ingreso exitoso.",
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Roles = roles
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UsuarioCreateRequest request)
        {
            try
            {
                var claveHash = PasswordHelper.HashPassword(request.Clave);

                _authRepository.CrearUsuario(
                    request.NombreUsuario,
                    claveHash,
                    request.RolId
                );

                return Ok(new
                {
                    mensaje = "Usuario creado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al crear usuario",
                    error = ex.Message
                });
            }
        }

    }
}