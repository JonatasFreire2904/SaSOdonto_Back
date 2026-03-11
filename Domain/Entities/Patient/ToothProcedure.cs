namespace Domain.Entities
{
    public class ToothProcedure
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ToothNumber { get; set; }
        public string Faces { get; set; } = string.Empty; // Ex: "V,L,M,D,O"
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Guid ProcedureId { get; set; }
        public Procedure Procedure { get; set; } = null!;
    }
}
