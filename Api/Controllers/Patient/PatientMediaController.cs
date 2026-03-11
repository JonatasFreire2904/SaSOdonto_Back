using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clinicas/{clinicId}/pacientes/{patientId}/midias")]
    public class PatientMediaController : ControllerBase
    {
        private readonly PatientMediaService _service;

        public PatientMediaController(PatientMediaService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid clinicId, Guid patientId)
        {
            var result = await _service.GetByPatientAsync(GetUserId(), clinicId, patientId);
            if (result == null) return Forbid();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Guid clinicId, Guid patientId, IFormFile file, [FromForm] string? name)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo é obrigatório.");

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("Arquivo deve ter no máximo 10MB.");

            var result = await _service.UploadAsync(GetUserId(), clinicId, patientId, file, name);
            if (result == null) return Forbid();
            return Created($"api/clinicas/{clinicId}/pacientes/{patientId}/midias/{result.Id}", result);
        }

        [HttpDelete("{mediaId}")]
        public async Task<IActionResult> Delete(Guid clinicId, Guid patientId, Guid mediaId)
        {
            var result = await _service.DeleteAsync(GetUserId(), clinicId, patientId, mediaId);
            if (result == null) return Forbid();
            if (result == false) return NotFound();
            return NoContent();
        }
    }
}
