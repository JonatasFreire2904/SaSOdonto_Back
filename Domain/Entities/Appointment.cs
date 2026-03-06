using Domain.Enums;

namespace Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Procedure { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        public decimal? Price { get; set; }
        public string? DentistName { get; set; }
        public string? Tooth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;

        /// <summary>
        /// Entrada automática gerada no prontuário ao concluir atendimento
        /// </summary>
        public MedicalRecord? MedicalRecord { get; set; }
    }
}
