using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Procedure;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clinicas/{clinicId}/procedimentos")]
    public class ProcedureController : ControllerBase
    {
        private readonly ProcedureService _service;

        public ProcedureController(ProcedureService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Lista todos os procedimentos da clínica (padrão + personalizados)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid clinicId)
        {
            var result = await _service.GetByClinicAsync(GetUserId(), clinicId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        /// <summary>
        /// Cria um procedimento personalizado na clínica
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Guid clinicId, [FromBody] CreateProcedureRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(GetUserId(), clinicId, request);
            if (result == null) return Forbid();
            return CreatedAtAction(nameof(GetAll), new { clinicId }, result);
        }

        /// <summary>
        /// Remove um procedimento da clínica
        /// </summary>
        [HttpDelete("{procedureId}")]
        public async Task<IActionResult> Delete(Guid clinicId, Guid procedureId)
        {
            var result = await _service.DeleteAsync(GetUserId(), clinicId, procedureId);
            if (result == null) return Forbid();
            if (!result.Value) return NotFound("Procedimento não encontrado.");
            return NoContent();
        }
    }
}
