using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonalController : ControllerBase
    {
        private readonly PersonalRepository _repository;

        public PersonalController(PersonalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repository.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _repository.ObtenerPorId(id);
            if (model == null) return NotFound(new { mensaje = "Registro no encontrado" });

            return Ok(model);
        }

        [HttpGet("{id}/detalle")]
        public IActionResult GetDetalle(int id)
        {
            var detalle = _repository.ObtenerDetalle(id);
            if (detalle == null) return NotFound(new { mensaje = "Registro no encontrado" });

            return Ok(detalle);
        }

        [HttpGet("barcos")]
        public IActionResult GetBarcos()
        {
            return Ok(_repository.ObtenerBarcosActivos());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Personal model)
        {
            if (_repository.ExisteIdentificacion(model.IdentificacionUnica))
            {
                return BadRequest(new { mensaje = "La identificación única ya existe." });
            }

            _repository.Crear(model);
            return Ok(new { mensaje = "Personal creado correctamente" });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Personal model)
        {
            if (_repository.ExisteIdentificacion(model.IdentificacionUnica, id))
            {
                return BadRequest(new { mensaje = "La identificación única ya existe." });
            }

            model.Id = id;
            _repository.Editar(model);
            return Ok(new { mensaje = "Personal actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.EliminarLogico(id);
            return Ok(new { mensaje = "Personal desactivado correctamente" });
        }
    }
}