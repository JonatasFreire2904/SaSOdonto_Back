using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Shared.DTOs.Patient
{
    public class UpdatePatientRequest
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(14)]
        public string? Cpf { get; set; }

        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
