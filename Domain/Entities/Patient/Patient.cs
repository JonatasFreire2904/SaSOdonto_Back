using Domain.Enums;

namespace Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<OdontogramEntry> OdontogramEntries { get; set; } = new List<OdontogramEntry>();
        public ICollection<PatientMedia> PatientMedias { get; set; } = new List<PatientMedia>();
        public Anamnesis? Anamnesis { get; set; }
        public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        public ICollection<ToothProcedure> ToothProcedures { get; set; } = new List<ToothProcedure>();
    }
}
