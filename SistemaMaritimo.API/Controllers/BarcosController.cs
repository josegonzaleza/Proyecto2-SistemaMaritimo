using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BarcosController : ControllerBase
    {
        private readonly BarcosRepository _repository;

        public BarcosController(BarcosRepository repository)
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
            if (model == null)
                return NotFound(new { mensaje = "Barco no encontrado." });

            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Barco model)
        {
            if (_repository.ExisteMatricula(model.MatriculaUnica))
            {
                return BadRequest(new { mensaje = "La matrícula única ya existe." });
            }

            _repository.Crear(model);
            return Ok(new { mensaje = "Barco creado correctamente." });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Barco model)
        {
            if (_repository.ExisteMatricula(model.MatriculaUnica, id))
            {
                return BadRequest(new { mensaje = "La matrícula única ya existe." });
            }

            model.Id = id;
            _repository.Editar(model);

            return Ok(new { mensaje = "Barco actualizado correctamente." });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.Archivar(id);
            return Ok(new { mensaje = "Barco archivado correctamente." });
        }

        [HttpPut("{id}/activar")]
        public IActionResult Activar(int id)
        {
            _repository.Activar(id);
            return Ok(new { mensaje = "Barco activado correctamente." });
        }
    }
}