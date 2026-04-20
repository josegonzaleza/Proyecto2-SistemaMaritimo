using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdenesServicioController : ControllerBase
    {
        private readonly OrdenesServicioRepository _repository;
        private readonly PersonalRepository _personalRepository;

        public OrdenesServicioController(OrdenesServicioRepository repository, PersonalRepository personalRepository)
        {
            _repository = repository;
            _personalRepository = personalRepository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? barcoId, [FromQuery] string? tecnicoId)
        {
            return Ok(_repository.ObtenerTodas(barcoId, tecnicoId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repository.ObtenerPorId(id);
            if (item == null)
                return NotFound(new { mensaje = "Orden no encontrada." });

            return Ok(item);
        }

        [HttpGet("tecnicos")]
        public IActionResult GetTecnicos()
        {
            return Ok(_repository.ObtenerTecnicos());
        }

        [HttpGet("historial/{barcoId}")]
        public IActionResult GetHistorial(int barcoId)
        {
            return Ok(_repository.ObtenerHistorialPorBarco(barcoId));
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrdenServicio model)
        {
            model.CodigoOrden = _repository.GenerarCodigoOrden();
            model.Estado = "Abierta";

            _repository.Crear(model);
            return Ok(new { mensaje = "Orden creada correctamente." });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] OrdenServicio model)
        {
            model.Id = id;
            _repository.Editar(model);
            return Ok(new { mensaje = "Orden actualizada correctamente." });
        }

        [HttpPut("{id}/asignar")]
        public IActionResult AsignarTecnico(int id, [FromBody] AsignarTecnicoRequest request)
        {
            _repository.AsignarTecnico(id, request.PersonalId);
            return Ok(new { mensaje = "Técnico asignado correctamente." });
        }

        [HttpPut("{id}/estado")]
        public IActionResult CambiarEstado(int id, [FromBody] CambiarEstadoOrdenRequest request)
        {
            _repository.CambiarEstado(id, request.NuevoEstado);
            return Ok(new { mensaje = "Estado actualizado correctamente." });
        }

        [HttpPut("{id}/cerrar")]
        public IActionResult Cerrar(int id, [FromBody] CerrarOrdenRequest request)
        {
            _repository.CerrarOrden(id, request.InformeCierre, request.UsuarioCierre);
            return Ok(new { mensaje = "Orden cerrada correctamente." });
        }
    }
}