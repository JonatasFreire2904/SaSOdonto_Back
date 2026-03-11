using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.MedicalRecord;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes/{patientId}/prontuario")]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _service;

        public MedicalRecordController(MedicalRecordService service)
        {
            _service = service;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Retorna todo o prontuário (histórico) do paciente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRecords(Guid clinicId, Guid patientId)
        {
            var userId = GetUserId();
            var records = await _service.GetByPatientAsync(userId, clinicId, patientId);

            if (records == null)
                return Forbid();

            return Ok(records);
        }

        /// <summary>
        /// Retorna apenas os alertas médicos (alergias e condições crônicas)
        /// </summary>
        [HttpGet("alertas")]
        public async Task<IActionResult> GetAlerts(Guid clinicId, Guid patientId)
        {
            var userId = GetUserId();
            var alerts = await _service.GetAlertsAsync(userId, clinicId, patientId);

            if (alerts == null)
                return Forbid();

            return Ok(alerts);
        }

        /// <summary>
        /// Adiciona entrada manual ao prontuário (alergia, observação, etc.)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRecord(Guid clinicId, Guid patientId, [FromBody] CreateMedicalRecordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var record = await _service.CreateAsync(userId, clinicId, patientId, request);

            if (record == null)
                return Forbid();

            return Created($"api/clinicas/{clinicId}/pacientes/{patientId}/prontuario/{record.Id}", record);
        }

        /// <summary>
        /// Remove uma entrada do prontuário
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(Guid clinicId, Guid patientId, Guid id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(userId, clinicId, patientId, id);

            if (deleted == null)
                return Forbid();

            if (!deleted.Value)
                return NotFound("Registro não encontrado.");

            return NoContent();
        }
    }
}
