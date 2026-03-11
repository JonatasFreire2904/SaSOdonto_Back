using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Patient;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Lista todos os pacientes de uma clínica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPatients(Guid clinicId, [FromQuery] string? q = null)
        {
            var userId = GetUserId();
            var patients = await _patientService.GetPatientsByClinicAsync(userId, clinicId, q);

            if (patients == null)
                return Forbid();

            return Ok(patients);
        }

        /// <summary>
        /// Retorna um paciente específico pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(Guid clinicId, Guid id)
        {
            var userId = GetUserId();
            var patient = await _patientService.GetPatientByIdAsync(userId, clinicId, id);

            if (patient == null)
                return NotFound("Paciente não encontrado.");

            return Ok(patient);
        }

        /// <summary>
        /// Cria um novo paciente na clínica
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePatient(Guid clinicId, [FromBody] CreatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var patient = await _patientService.CreatePatientAsync(userId, clinicId, request);

            if (patient == null)
                return Forbid();

            return CreatedAtAction(nameof(GetPatient), new { clinicId, id = patient.Id }, patient);
        }

        /// <summary>
        /// Atualiza os dados de um paciente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid clinicId, Guid id, [FromBody] UpdatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var patient = await _patientService.UpdatePatientAsync(userId, clinicId, id, request);

            if (patient == null)
                return NotFound("Paciente não encontrado.");

            return Ok(patient);
        }

        /// <summary>
        /// Remove um paciente da clínica
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid clinicId, Guid id)
        {
            var userId = GetUserId();
            var deleted = await _patientService.DeletePatientAsync(userId, clinicId, id);

            if (deleted == null)
                return Forbid();

            if (!deleted.Value)
                return NotFound("Paciente não encontrado.");

            return NoContent();
        }
    }
}
