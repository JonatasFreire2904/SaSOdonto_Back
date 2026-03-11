namespace Domain.Entities
{
    public class Procedure
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = null!;
    }
}
