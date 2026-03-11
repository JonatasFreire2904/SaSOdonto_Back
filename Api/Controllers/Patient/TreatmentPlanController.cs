using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.TreatmentPlan;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes/{patientId}/plano-tratamento")]
    public class TreatmentPlanController : ControllerBase
    {
        private readonly TreatmentPlanService _service;

        public TreatmentPlanController(TreatmentPlanService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Lista todos os itens do plano de tratamento de um paciente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid clinicId, Guid patientId)
        {
            var result = await _service.GetByPatientAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        /// <summary>
        /// Adiciona um item ao plano de tratamento
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Guid clinicId, Guid patientId, [FromBody] CreateTreatmentPlanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(GetUserId(), clinicId, patientId, request);
            if (result == null) return Forbid();
            return CreatedAtAction(nameof(GetAll), new { clinicId, patientId }, result);
        }

        /// <summary>
        /// Atualiza um item do plano de tratamento
        /// </summary>
        [HttpPut("{planId}")]
        public async Task<IActionResult> Update(Guid clinicId, Guid patientId, Guid planId, [FromBody] UpdateTreatmentPlanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(GetUserId(), clinicId, patientId, planId, request);
            if (result == null) return NotFound("Item do plano de tratamento não encontrado.");
            return Ok(result);
        }

        /// <summary>
        /// Remove um item específico do plano de tratamento
        /// </summary>
        [HttpDelete("{planId}")]
        public async Task<IActionResult> Delete(Guid clinicId, Guid patientId, Guid planId)
        {
            var result = await _service.DeleteAsync(GetUserId(), clinicId, patientId, planId);
            if (result == null) return Forbid();
            if (!result.Value) return NotFound("Item do plano de tratamento não encontrado.");
            return NoContent();
        }

        /// <summary>
        /// Remove todo o plano de tratamento do paciente
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAll(Guid clinicId, Guid patientId)
        {
            var result = await _service.DeleteAllByPatientAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            if (!result.Value) return NotFound("Plano de tratamento não encontrado.");
            return NoContent();
        }
    }
}
