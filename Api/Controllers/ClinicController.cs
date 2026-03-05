using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Clinic;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/clinicas")]
    [Authorize]
    public class ClinicController : ControllerBase
    {
        private readonly ClinicService _clinicService;

        public ClinicController(ClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Retorna todas as clínicas do usuário logado (como dono + como membro/parceiro)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetClinics()
        {
            var userId = GetUserId();
            var clinics = await _clinicService.GetClinicsByUserAsync(userId);
            return Ok(clinics);
        }

        /// <summary>
        /// Cria uma nova clínica e associa ao usuário logado
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var clinic = await _clinicService.CreateClinicAsync(userId, request);
            return CreatedAtAction(nameof(GetClinics), new { id = clinic.Id }, clinic);
        }

        /// <summary>
        /// Define a clínica ativa para a sessão do usuário
        /// </summary>
        [HttpPost("{id}/selecionar")]
        public async Task<IActionResult> SelectClinic(Guid id)
        {
            var userId = GetUserId();
            var clinic = await _clinicService.SelectClinicAsync(userId, id);

            if (clinic == null)
                return NotFound("Clínica não encontrada ou você não tem acesso.");

            return Ok(clinic);
        }
    }
}
