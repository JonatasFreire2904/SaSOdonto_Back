using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Anamnesis;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes/{patientId}/anamnese")]
    public class AnamnesisController : ControllerBase
    {
        private readonly AnamnesisService _service;

        public AnamnesisController(AnamnesisService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Retorna a anamnese do paciente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(Guid clinicId, Guid patientId)
        {
            var result = await _service.GetByPatientAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        /// <summary>
        /// Salva/atualiza a anamnese do paciente (cria se não existir, atualiza se já existir)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Save(Guid clinicId, Guid patientId, [FromBody] SaveAnamnesisRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.SaveAsync(GetUserId(), clinicId, patientId, request);
            if (result == null) return Forbid();
            return Ok(result);
        }
    }
}
