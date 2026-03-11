using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Procedure
{
    public class CreateProcedureRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;
    }
}
