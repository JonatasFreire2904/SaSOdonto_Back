using Domain.Enums;

namespace Domain.Entities
{
    public class OdontogramEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ToothNumber { get; set; }
        public ToothStatus Status { get; set; } = ToothStatus.Normal;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
