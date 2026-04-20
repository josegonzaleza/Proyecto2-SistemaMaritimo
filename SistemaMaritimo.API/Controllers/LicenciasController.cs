using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/personal/{personalId}/[controller]")]
    [Authorize]
    public class LicenciasController : ControllerBase
    {
        private readonly LicenciasRepository _repository;

        public LicenciasController(LicenciasRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get(int personalId)
        {
            return Ok(_repository.ObtenerPorPersonal(personalId));
        }

        [HttpPost]
        public IActionResult Post(int personalId, [FromBody] LicenciaMaritima model)
        {
            model.PersonalId = personalId;
            _repository.Crear(model);
            return Ok(new { mensaje = "Licencia registrada correctamente" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int personalId, int id)
        {
            _repository.Eliminar(id);
            return Ok(new { mensaje = "Licencia eliminada correctamente" });
        }
    }
}