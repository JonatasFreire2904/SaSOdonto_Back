using Domain.Enums;

namespace Shared.DTOs.Appointment
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public string Procedure { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal? Price { get; set; }
        public string? DentistName { get; set; }
        public string? Tooth { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
