namespace Domain.Entities
{
    public class TreatmentPlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
