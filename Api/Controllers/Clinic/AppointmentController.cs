using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Appointment;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/clinicas/{clinicId}/atendimentos")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _service;

        public AppointmentController(AppointmentService service)
        {
            _service = service;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Lista todos os atendimentos da clínica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAppointments(Guid clinicId)
        {
            var userId = GetUserId();
            var appointments = await _service.GetByClinicAsync(userId, clinicId);

            if (appointments == null)
                return Forbid();

            return Ok(appointments);
        }

        /// <summary>
        /// Lista atendimentos de um paciente específico
        /// </summary>
        [HttpGet("paciente/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid clinicId, Guid patientId)
        {
            var userId = GetUserId();
            var appointments = await _service.GetByPatientAsync(userId, clinicId, patientId);

            if (appointments == null)
                return Forbid();

            return Ok(appointments);
        }

        /// <summary>
        /// Cria um novo atendimento (agendamento)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Guid clinicId, [FromBody] CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var appointment = await _service.CreateAsync(userId, clinicId, request);

            if (appointment == null)
                return Forbid();

            return CreatedAtAction(nameof(GetAppointments), new { clinicId }, appointment);
        }

        /// <summary>
        /// Conclui um atendimento (gera entrada automática no prontuário)
        /// </summary>
        [HttpPost("{id}/concluir")]
        public async Task<IActionResult> CompleteAppointment(Guid clinicId, Guid id, [FromBody] CompleteAppointmentRequest request)
        {
            var userId = GetUserId();
            var appointment = await _service.CompleteAsync(userId, clinicId, id, request);

            if (appointment == null)
                return NotFound("Atendimento não encontrado.");

            return Ok(appointment);
        }

        /// <summary>
        /// Cancela um atendimento
        /// </summary>
        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelAppointment(Guid clinicId, Guid id)
        {
            var userId = GetUserId();
            var appointment = await _service.CancelAsync(userId, clinicId, id);

            if (appointment == null)
                return NotFound("Atendimento não encontrado.");

            return Ok(appointment);
        }
    }
}
