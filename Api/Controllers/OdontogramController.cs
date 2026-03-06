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
    }
}
