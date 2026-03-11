using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Odontogram;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes/{patientId}/odontograma")]
    public class OdontogramController : ControllerBase
    {
        private readonly OdontogramService _service;

        public OdontogramController(OdontogramService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Get(Guid clinicId, Guid patientId)
        {
            var result = await _service.GetByPatientAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        [HttpPut("{toothNumber}")]
        public async Task<IActionResult> UpdateTooth(Guid clinicId, Guid patientId, int toothNumber, UpdateToothRequest request)
        {
            var result = await _service.UpdateToothAsync(GetUserId(), clinicId, patientId, toothNumber, request);
            if (result == null) return Forbid();
            return Ok(result);
        }

        /// <summary>
        /// Adiciona um procedimento a um dente
        /// </summary>
        [HttpPost("procedimentos")]
        public async Task<IActionResult> AddToothProcedure(Guid clinicId, Guid patientId, [FromBody] CreateToothProcedureRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddToothProcedureAsync(GetUserId(), clinicId, patientId, request);
            if (result == null) return Forbid();
            return Created("", result);
        }

        /// <summary>
        /// Lista todos os procedimentos realizados nos dentes do paciente
        /// </summary>
        [HttpGet("procedimentos")]
        public async Task<IActionResult> GetToothProcedures(Guid clinicId, Guid patientId)
        {
            var result = await _service.GetToothProceduresAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        /// <summary>
        /// Remove um procedimento de um dente
        /// </summary>
        [HttpDelete("procedimentos/{toothProcedureId}")]
        public async Task<IActionResult> DeleteToothProcedure(Guid clinicId, Guid patientId, Guid toothProcedureId)
        {
            var result = await _service.DeleteToothProcedureAsync(GetUserId(), clinicId, patientId, toothProcedureId);
            if (result == null) return Forbid();
            if (!result.Value) return NotFound("Procedimento não encontrado.");
            return NoContent();
        }
    }
}
