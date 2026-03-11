using Domain.Enums;

namespace Domain.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public MedicalRecordType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        /// <summary>
        /// Se o registro foi gerado por um atendimento, referencia ele aqui
        /// </summary>
        public Guid? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
