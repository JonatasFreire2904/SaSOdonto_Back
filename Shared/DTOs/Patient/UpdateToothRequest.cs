using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Shared.DTOs.Odontogram
{
    public class UpdateToothRequest
    {
        [Required]
        public ToothStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
