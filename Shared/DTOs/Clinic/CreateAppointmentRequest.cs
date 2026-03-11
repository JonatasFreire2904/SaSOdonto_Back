using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Appointment
{
    public class CreateAppointmentRequest
    {
        [Required, MaxLength(200)]
        public string Procedure { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        public decimal? Price { get; set; }

        [MaxLength(200)]
        public string? DentistName { get; set; }

        [MaxLength(50)]
        public string? Tooth { get; set; }

        [Required]
        public Guid PatientId { get; set; }
    }
}
