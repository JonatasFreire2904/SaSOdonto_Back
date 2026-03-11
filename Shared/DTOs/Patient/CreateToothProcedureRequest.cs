using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Odontogram
{
    public class CreateToothProcedureRequest
    {
        [Required]
        public int ToothNumber { get; set; }

        [Required]
        public Guid ProcedureId { get; set; }

        [Required, MaxLength(50)]
        public string Faces { get; set; } = string.Empty; // Ex: "V,L,M,D,O"

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
