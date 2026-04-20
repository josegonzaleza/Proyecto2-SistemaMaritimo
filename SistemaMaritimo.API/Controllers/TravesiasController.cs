using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TravesiasController : ControllerBase
    {
        private readonly TravesiasRepository _repository;

        public TravesiasController(TravesiasRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? barcoId, [FromQuery] string? puertoDestino)
        {
            return Ok(_repository.ObtenerTodas(barcoId, puertoDestino));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repository.ObtenerPorId(id);
            if (item == null)
                return NotFound(new { mensaje = "Travesía no encontrada." });

            return Ok(item);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Travesia model)
        {
            _repository.Crear(model);
            return Ok(new { mensaje = "Travesía creada correctamente." });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Travesia model)
        {
            model.Id = id;
            _repository.Editar(model);
            return Ok(new { mensaje = "Travesía actualizada correctamente." });
        }

        [HttpPut("{id}/estado")]
        public IActionResult CambiarEstado(int id, [FromBody] CambiarEstadoTravesiaRequest request)
        {
            _repository.CambiarEstado(id, request.NuevoEstado, request.Usuario);
            return Ok(new { mensaje = "Estado actualizado correctamente." });
        }
    }
}