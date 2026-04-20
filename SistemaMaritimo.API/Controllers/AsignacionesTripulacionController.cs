using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMaritimo.API.Models;
using SistemaMaritimo.API.Repositories;

namespace SistemaMaritimo.API.Controllers
{
    [ApiController]
    [Route("api/personal/{personalId}/[controller]")]
    [Authorize]
    public class AsignacionesTripulacionController : ControllerBase
    {
        private readonly AsignacionesTripulacionRepository _repository;

        public AsignacionesTripulacionController(AsignacionesTripulacionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get(int personalId)
        {
            return Ok(_repository.ObtenerPorPersonal(personalId));
        }

        [HttpPost]
        public IActionResult Post(int personalId, [FromBody] AsignacionTripulacion model)
        {
            model.PersonalId = personalId;
            _repository.Crear(model);
            return Ok(new { mensaje = "Asignación registrada correctamente" });
        }
    }
}